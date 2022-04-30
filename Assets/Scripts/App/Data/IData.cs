
namespace CBK.Product.Data
{
    public interface IData
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize();

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy();
    }
}