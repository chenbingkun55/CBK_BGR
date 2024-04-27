using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace CBK.Framework.Res
{
    public interface IResCore
    {
        /// <summary>
        /// 同步加载指定资源
        /// </summary>
        T LoadSync<T>(string assetKey) where T : Object;

        /// <summary>
        /// 同步加载指定原生文件的二进制数据
        /// </summary>
        byte[] LoadRawSync(string assetKey);

        /// <summary>
        /// 同步加载指定原生文件的文本数据
        /// </summary>
        string LoadRawTextSync(string assetKey);

        /// <summary>
        /// 异步加载指定资源
        /// </summary>
        UniTask<T> LoadAsync<T>(string assetKey) where T : Object;

        /// <summary>
        /// 异步加载指定原生文件的二进制数据
        /// </summary>
        UniTask<byte[]> LoadRawAsync(string assetKey);

        /// <summary>
        /// 异步加载指定原生文件的文本数据
        /// </summary>
        UniTask<string> LoadRawTextAsync(string assetKey);

        /// <summary>
        /// 释放对指定资源的引用
        /// </summary>
        void Release(Object asset);

        /// <summary>
        /// 卸载未使用的资源
        /// </summary>
        void UnloadUnusedResources();

        /// <summary>
        /// 卸载通过该接口加载的所有资源
        /// </summary>
        void UnloadResources();
    }
}