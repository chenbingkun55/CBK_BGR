using System.Collections.Generic;
using System.Text;

namespace FairyGUI.Utils
{
    public static class StringUtils
    {
        public delegate string ParseTemplateDelegate(string template, IReadOnlyDictionary<string, string> dictVars);
        public static ParseTemplateDelegate ParseTemplateHandler { get; set; } = ParseTemplate;
        
        private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

        /// <summary>
        /// 判断指定字符串是否是FairyGUI的模板字符串
        /// </summary>
        private static bool IsStringTemplateStr(string str)
        {
            return !string.IsNullOrEmpty(str) && str.IndexOf('{') >= 0;
        }

        /// <summary>
        /// 符合FairyGUI模板语法的字符串替换方法
        /// </summary>
        public static string ParseTemplate(string template, IReadOnlyDictionary<string, string> dictVars)
        {
            if (!IsStringTemplateStr(template))
                return template;
            
            int pos1 = 0, pos2 = 0;
            int pos3;
            string tag;
            string value;
            sharedStringBuilder.Clear();

            while ((pos2 = template.IndexOf('{', pos1)) != -1)
            {
                if (pos2 > 0 && template[pos2 - 1] == '\\')
                {
                    sharedStringBuilder.Append(template, pos1, pos2 - pos1 - 1);
                    sharedStringBuilder.Append('{');
                    pos1 = pos2 + 1;
                    continue;
                }

                sharedStringBuilder.Append(template, pos1, pos2 - pos1);
                pos1 = pos2;
                pos2 = template.IndexOf('}', pos1);
                if (pos2 == -1)
                    break;

                if (pos2 == pos1 + 1)
                {
                    sharedStringBuilder.Append(template, pos1, 2);
                    pos1 = pos2 + 1;
                    continue;
                }

                tag = template.Substring(pos1 + 1, pos2 - pos1 - 1);
                pos3 = tag.IndexOf('=');
                if (pos3 != -1)
                {
                    if (!dictVars.TryGetValue(tag.Substring(0, pos3), out value))
                        value = tag.Substring(pos3 + 1);
                }
                else
                {
                    if (!dictVars.TryGetValue(tag, out value))
                        value = string.Empty;
                }

                sharedStringBuilder.Append(value);
                pos1 = pos2 + 1;
            }

            if (pos1 < template.Length)
                sharedStringBuilder.Append(template, pos1, template.Length - pos1);

            return sharedStringBuilder.ToString();
        }
    }
}