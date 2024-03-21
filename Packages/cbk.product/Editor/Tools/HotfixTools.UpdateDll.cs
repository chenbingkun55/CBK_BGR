using UnityEngine;
using UnityEditor;
using CBK.Mono;
using System.IO;
using UnityEngine.AddressableAssets;

namespace CBK.Editor
{
    /// <summary>
    /// Hotfix工具
    /// </summary>
    public sealed partial class HotfixTools : UnityEditor.Editor
    {
        private static readonly string hybridCLRPath = $"{Application.dataPath}/../HybridCLRData/HotUpdateDlls/StandaloneWindows64";
        private static readonly string hotfixDllPath = $"{Application.dataPath}/../Assets/ProductAssets/HotfixDll";

        /// <summary>
        /// 复制dll到 Assets\ProductAssets\HotfixDll
        /// </summary>
        [MenuItem("Tools/Hotfix/UpdateDll")]
        static void UpdateDll()
        {
            Debug.Log("Update Dll Start ...");
            Debug.Log($"HybridCLR Path: {hybridCLRPath}");
            Debug.Log($"HotfixDll Path: {hotfixDllPath}");

            var config = Addressables.LoadAssetAsync<HotfixDllConfig>("ScriptableObject/HotfixDllConfig.asset").WaitForCompletion();
            foreach (var dll in config.hotfixDlls)
            {
                var soruce = $"{hybridCLRPath}/{dll}";
                var target = $"{hotfixDllPath}/{dll}.bytes";

                File.Copy(soruce, target, true);
                Debug.Log($"Update Dll From: {soruce}\n \t\t To: {target}");
            }

            Debug.Log("Update Dll End");
        }
    }
}