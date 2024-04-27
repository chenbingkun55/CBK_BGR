namespace CBK.Framework.Core
{
    /// <summary>
    /// 初始化接口 实现该接口的类将会OnResolving阶段被调用
    /// </summary>
    public interface IInitialize
    {
        void Initialize();
    }
}