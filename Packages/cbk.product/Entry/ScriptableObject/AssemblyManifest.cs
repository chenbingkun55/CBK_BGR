using System;
using System.Collections.Generic;
using UnityEngine;

namespace CBK.Entry
{
    /// <summary>
    /// AssemblyManifest 配置
    /// </summary>
    [CreateAssetMenu(menuName = "CBK Config/" + nameof(AssemblyManifest), fileName = nameof(AssemblyManifest), order = 0)]
    public class AssemblyManifest : ScriptableObject
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version = "v1.0.0";

        /// <summary>
        /// 热更新程序集
        /// </summary>
        public string[] hotfixAssemblyNames;

        /// <summary>
        /// AOT补充元数据程序集
        /// </summary>
        public string[] aotPatchAssemblyNames;

        /// <summary>
        /// AOT补充元数据程序集资源键值字符串格式化参数
        /// </summary>
        public string aotPatchAssemblyAssetKeyFormat = "{0}";

        /// <summary>
        /// 逻辑程序集入口类名
        /// </summary>
        public string logicEntryTypeName = "CBK.Logic.LogicEntry";

        /// <summary>
        /// 逻辑程序集入口方法名
        /// </summary>
        public string logicEntryStaticMethodName = "Init";

        /// <summary>
        /// 获取AOT补充元数据程序集资源键值
        /// </summary>
        public string GetAOTPatchAssemblyAssetKey(string assemblyName)
        {
            return string.Format(aotPatchAssemblyAssetKeyFormat, assemblyName);
        }

#if UNITY_EDITOR
        public static AssemblyManifest GetOrCreateManifest()
        {
            const string path = "Assets/HybridCLRData/GameAssemblyManifest.asset";
            var manifest = UnityEditor.AssetDatabase.LoadAssetAtPath<AssemblyManifest>(path);
            if (manifest != null)
                return manifest;

            manifest = ScriptableObject.CreateInstance<AssemblyManifest>();

            UnityEditor.AssetDatabase.CreateAsset(manifest, path);
            UnityEditor.AssetDatabase.SaveAssets();

            return manifest;
        }
#endif
    }
}