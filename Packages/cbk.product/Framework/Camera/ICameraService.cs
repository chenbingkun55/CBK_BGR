using System.Collections.Generic;

namespace CBK.Framework.Camera
{
    /// <summary>
    /// 相机服务接口
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// 获取当前相机栈上的相机
        /// </summary>
        int GetCamerasInStack(List<UnityEngine.Camera> buffer);
    }
}