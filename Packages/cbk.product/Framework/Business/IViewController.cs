using System;

namespace CBK.Framework.Business
{
    /// <summary>
    /// 视图控制器接口
    /// </summary>
    public interface IViewController : IDisposable
    {
        void Initialize();
    }
}