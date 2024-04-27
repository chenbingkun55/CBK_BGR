using System.Collections.Generic;

namespace CBK.Framework.DynamicText
{
    public static class DynamicTextServiceExtensions
    {
        /// <summary>
        /// 设置文本变量 并返回当前实例
        /// </summary>
        public static IDynamicTextCore SetVar(this string source, string name, string var)
        {
            return DynamicTextService.That.Allocate(source)
                .SetVar(name, var);
        }

        /// <summary>
        /// 设置文本变量 并返回当前实例
        /// </summary>
        public static IDynamicTextCore SetVars(this string source, IReadOnlyDictionary<string, string> vars)
        {
            var core = DynamicTextService.That.Allocate(source);

            if (vars == null)
                return core;
            
            foreach (var pair in vars)
                core.SetVar(pair.Key, pair.Value);
            
            return core;
        }
    }
}