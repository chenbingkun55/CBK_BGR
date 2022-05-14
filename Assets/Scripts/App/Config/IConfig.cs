using System.Collections;

namespace CBK.Product.Config
{
    public interface IConfig
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize();

        /// <summary>
        /// 初始化异步
        /// </summary>
        /// <returns></returns>
        public IEnumerable InitializeAsync();

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy();
    }
}