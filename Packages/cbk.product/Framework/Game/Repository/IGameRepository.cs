using CatLib;

namespace CBK.Framework.Game
{
    /// <summary>
    /// 玩法仓库接口
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// 清空仓库
        /// </summary>
        void Clear();
    }

    public abstract class AGameRepository<T> : Facade<T>, IGameRepository where T : AGameRepository<T>
    {
        public abstract void Clear();
    }
}