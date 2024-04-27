namespace CBK.Framework.Update
{
    /// <summary>
    /// 拥有帧更新能力的接口 由IUpdateService驱动
    /// </summary>
    public interface IFixedUpdate
    {
        void OnFixedUpdate(float elapseSeconds, float realElapseSeconds);
    }
}