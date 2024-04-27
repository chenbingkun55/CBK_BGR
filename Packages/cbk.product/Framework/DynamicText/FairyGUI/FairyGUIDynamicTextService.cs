using System;
using System.Collections.Generic;
using System.Text;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Reference;
using CBK.Framework.Reflect;

namespace CBK.Framework.DynamicText
{
    /// <summary>
    /// 与FairyGUI相同实现的动态文本服务
    /// </summary>
    internal sealed class FairyGUIDynamicTextService : IInitialize, IDynamicTextService
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }
        
        [Inject]
        public IReflectService ReflectService { get; set; }
        
        private readonly Dictionary<string, IDynamicTextVarParser> m_Parsers = new Dictionary<string, IDynamicTextVarParser>();
        private readonly Dictionary<string, IDynamicTextVarParamParser> m_ParamParsers = new Dictionary<string, IDynamicTextVarParamParser>();
        
        public void Initialize()
        {
            ReflectService.ForEachType(typeof(IDynamicTextVarParser), type =>
            {
                var parser = (IDynamicTextVarParser) Activator.CreateInstance(type);
                if (m_Parsers.ContainsKey(parser.VarName))
                    Log.Error($"DynamicTextVarParser {parser.VarName} is already exist!");
                
                m_Parsers[parser.VarName] = parser;
            });
            
            ReflectService.ForEachType(typeof(IDynamicTextVarParamParser), type =>
            {
                var parser = (IDynamicTextVarParamParser) Activator.CreateInstance(type);
                if (m_ParamParsers.ContainsKey(parser.VarName))
                    Log.Error($"DynamicTextVarParamParser {parser.VarName} is already exist!");
                
                m_ParamParsers[parser.VarName] = parser;
            });
        }

        public IDynamicTextCore Allocate(string source)
        {
            var core = ReferenceService.GetReference<DynamicTextCore>();
            core.Source = source;
            return core;
        }
        
        private bool IsStringTemplateStr(string str)
        {
            return !string.IsNullOrEmpty(str) && str.IndexOf('{') >= 0;
        }

        private bool FlushVars(StringBuilder stringBuilder, string source, IReadOnlyDictionary<string, string> dictVars)
        {
            stringBuilder.Clear();
            if (!IsStringTemplateStr(source))
                return false;
            
            int pos1 = 0, pos2 = 0;
            int pos3;
            string tag;
            string value;
            stringBuilder.Clear();

            while ((pos2 = source.IndexOf('{', pos1)) != -1)
            {
                if (pos2 > 0 && source[pos2 - 1] == '\\')
                {
                    stringBuilder.Append(source, pos1, pos2 - pos1 - 1);
                    stringBuilder.Append('{');
                    pos1 = pos2 + 1;
                    continue;
                }

                stringBuilder.Append(source, pos1, pos2 - pos1);
                pos1 = pos2;
                pos2 = source.IndexOf('}', pos1);
                if (pos2 == -1)
                    break;

                if (pos2 == pos1 + 1)
                {
                    stringBuilder.Append(source, pos1, 2);
                    pos1 = pos2 + 1;
                    continue;
                }

                tag = source.Substring(pos1 + 1, pos2 - pos1 - 1);
                pos3 = tag.IndexOf('=');
                if (pos3 != -1)
                {
                    if (!ParseVar(dictVars, tag.Substring(0, pos3), out value))
                        value = tag.Substring(pos3 + 1);
                }
                else
                {
                    if (!ParseVar(dictVars, tag, out value))
                        value = string.Empty;
                }

                stringBuilder.Append(value);
                pos1 = pos2 + 1;
            }

            if (pos1 < source.Length)
                stringBuilder.Append(source, pos1, source.Length - pos1);

            return true;
        }
        
        private bool ParseVar(IReadOnlyDictionary<string, string> dictVars, string varName, out string str)
        {
            // 先从字典中获取
            if (dictVars.TryGetValue(varName, out str))
                return true;
            
            // 然后从DynamicTextVarParser中获取
            if (m_Parsers.TryGetValue(varName, out var parser))
            {
                str = parser.Parse();
                return true;
            }
            
            // 最后判断是否有带参数
            var indexOf = varName.IndexOf('_');
            if (indexOf > 0 && indexOf < varName.Length - 1 && m_ParamParsers.TryGetValue(varName.Substring(0, indexOf), out var paramParser))
            {
                // 从DynamicTextVarParamParser中获取
                str = paramParser.Parse(varName.Substring(indexOf + 1));
                return true;
            }

            str = string.Empty;
            return false;
        }

        private sealed class DynamicTextCore : AReference, IDynamicTextCore
        {
            public string Source { get; set; }
            public FairyGUIDynamicTextService Service { get; set; }
            
            private readonly Dictionary<string, string> m_Vars = new Dictionary<string, string>();
            private readonly StringBuilder m_StringBuilder = new StringBuilder();
            
            public override void OnRecycle()
            {
                Source = string.Empty;
                Service = null;
                
                m_Vars.Clear();
                m_StringBuilder.Clear();
            }

            public void Dispose()
            {
                this.Recycle();
            }

            public IDynamicTextCore SetVar(string name, string value)
            {
                m_Vars[name] = value;
                return this;
            }

            public string FlushVars(bool dispose = true)
            {
                var result = Service.FlushVars(m_StringBuilder, Source, m_Vars) ? m_StringBuilder.ToString() : Source;
                
                if (dispose)
                    this.Dispose();

                return result;
            }
        }
    }
}