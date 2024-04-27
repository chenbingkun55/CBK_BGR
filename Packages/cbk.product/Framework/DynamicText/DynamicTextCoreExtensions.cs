using System.Collections.Generic;

namespace CBK.Framework.DynamicText
{
    public static class DynamicTextCoreExtensions
    {
        public static IDynamicTextCore SetVars(this IDynamicTextCore core, IReadOnlyDictionary<string, string> dictVars)
        {
            foreach (var pair in dictVars)
                core.SetVar(pair.Key, pair.Value);
            
            return core;
        }
    }
}