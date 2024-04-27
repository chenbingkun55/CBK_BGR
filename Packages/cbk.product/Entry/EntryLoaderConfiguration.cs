using UnityEngine;

namespace CBK.Entry
{
    [CreateAssetMenu( menuName = "CBK Config/" + nameof(EntryLoaderConfiguration), fileName = nameof(EntryLoaderConfiguration))]
    public sealed class EntryLoaderConfiguration : ScriptableObject
    {
        /// <summary>
        /// 资源包名
        /// </summary>
        public string assetPackageName = "MainAssets";

        /// <summary>
        /// 资源包主文件服务器
        /// </summary>
        public string mainHostServer;

        /// <summary>
        /// 资源包备用文件服务器
        /// </summary>
        public string fallbackHostServer;

        /// <summary>
        /// 资源包解密服务类名
        /// </summary>
        public string decryptionServiceTypeName;

        /// <summary>
        /// 程序集清单资源键值
        /// </summary>
        public string assemblyManifestAssetKey = "AssemblyManifest";
    }
}