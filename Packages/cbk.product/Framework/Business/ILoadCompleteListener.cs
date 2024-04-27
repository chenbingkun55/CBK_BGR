namespace CBK.Framework.Business
{
    /// <summary>
    /// 需要关心流程加载完成的接口
    /// </summary>
    public interface ILoadCompleteListener
    {
        void OnLoadComplete();
    }
}