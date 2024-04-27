using System;

namespace CBK.Framework.UI.FairyGUI
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FairyGUIComponentAttribute : Attribute
    {
        public string URL { get; }

        public FairyGUIComponentAttribute(string url)
        {
            URL = url;
        }
    }
}