using System;

namespace CBK.Framework.Update
{
    /// <summary>
    /// 帧更新服务
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// 注册帧更新实例
        /// </summary>
        void RegisterUpdate(IUpdate update);

        /// <summary>
        /// 取消注册帧更新实例
        /// </summary>
        void UnRegisterUpdate(IUpdate update);
        
        /// <summary>
        /// 注册帧更新实例
        /// </summary>
        void RegisterLateUpdate(ILateUpdate update);

        /// <summary>
        /// 取消注册帧更新实例
        /// </summary>
        void UnRegisterLateUpdate(ILateUpdate update);
        
        /// <summary>
        /// 注册帧更新实例
        /// </summary>
        void RegisterFixedUpdate(IFixedUpdate update);

        /// <summary>
        /// 取消注册帧更新实例
        /// </summary>
        void UnRegisterFixedUpdate(IFixedUpdate update);

        /// <summary>
        /// 注册下一帧执行的回调
        /// </summary>
        void RegisterDelayCall(Action action);

        /// <summary>
        /// 取消注册下一帧执行的回调
        /// </summary>
        void UnRegisterDelayCall(Action action);
    }
}