namespace CBK.Framework.Res
{
    /// <summary>
    /// 资源服务
    /// </summary>
    public interface IResService : IResCore
    {
        /// <summary>
        /// 创建资源加载器 使用者需要在使用完毕后调用Dispose方法释放资源加载器
        /// </summary>
        IResLoader Allocate();
    }
}