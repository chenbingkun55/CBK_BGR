using CBK.Framework.Core;
using UnityEngine;

namespace CBK.Framework.ScreenAdaptor
{
    /// <summary>
    /// 屏幕适配服务接口。
    /// </summary>
    public interface IScreenAdaptorService
    {
        /// <summary>
        /// 获取安全区域范围
        /// </summary>
        ObservableVariable<Rect> SafeArea { get; }
    }
}