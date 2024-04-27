using System.Collections.Generic;

namespace CBK.Framework.Camera.Standard
{
    /// <summary>
    /// 基于 Unity 的标准相机服务
    /// </summary>
    public sealed class StandardCameraService : ICameraService
    {
        private UnityEngine.Camera[] m_CameraBuffer = new UnityEngine.Camera[8];
        
        public int GetCamerasInStack(List<UnityEngine.Camera> buffer)
        {
            buffer.Clear();
            
            var count = UnityEngine.Camera.allCamerasCount;
            if (m_CameraBuffer.Length < count)
                m_CameraBuffer = new UnityEngine.Camera[count];
            
            count = UnityEngine.Camera.GetAllCameras(m_CameraBuffer);

            for (var i = 0; i < count; i++)
            {
                var camera = m_CameraBuffer[i];
                if (!camera.enabled)
                    continue;
                
                if (camera.targetTexture != null)
                    continue;
                
                buffer.Add(camera);
            }
            
            buffer.Sort(SortByCameraDepth);
            
            return buffer.Count;
        }

        private int SortByCameraDepth(UnityEngine.Camera x, UnityEngine.Camera y)
        {
            return x.depth.CompareTo(y.depth);
        }
    }
}