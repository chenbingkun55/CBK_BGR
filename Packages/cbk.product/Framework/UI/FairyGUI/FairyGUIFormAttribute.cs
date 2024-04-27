using System;

namespace CBK.Framework.UI.FairyGUI
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FairyGUIFormAttribute : Attribute
    {
        public string PackageName { get; }
        public string ComponentName { get; }
        
        public string UIKey { get; }

        public FairyGUIFormAttribute(string packageName, string componentName)
        {
            UIKey = $"{packageName}.{componentName}";
            PackageName = packageName;
            ComponentName = componentName;
        }
    }
}