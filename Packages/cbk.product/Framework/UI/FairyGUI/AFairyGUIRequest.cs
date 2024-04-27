using System.Reflection;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI UI请求抽象类
    /// </summary>
    public abstract class AFairyGUIRequest<T> : AUIRequest where T: AFairyGUIFormLogic
    {
        protected sealed override string UIKey { get; }

        protected AFairyGUIRequest()
        {
            var attribute = typeof(T).GetCustomAttribute<FairyGUIFormAttribute>();
            if (attribute == null)
            {
                Log.Error($"UIKey is null, type: {typeof(T).FullName}");
                return;
            }
            
            UIKey = attribute.UIKey;
        }
    }
}