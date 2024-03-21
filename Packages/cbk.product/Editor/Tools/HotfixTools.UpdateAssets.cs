using UnityEngine;
using UnityEditor;
using System.IO;
using CBK.Editor.Utility;

namespace CBK.Editor
{
    /// <summary>
    /// Hotfix工具
    /// </summary>
    public partial class HotfixTools
    {
        private static readonly string sourcePath = $"{Application.dataPath}/../Library/com.unity.addressables/aa/Windows";
        private static readonly string targetPath = $"{Application.dataPath}/../Build/CBK_BGR_Data/StreamingAssets/aa";

        /// <summary>
        /// 更新资源到Build
        /// </summary>
        [MenuItem("Tools/Hotfix/UpdateAssets")]
        static void UpdateAssets()
        {
            Debug.Log("Update Assets Start ...");
            Debug.Log($"source Path: {sourcePath}");
            Debug.Log($"target Path: {targetPath}");

            Debug.Log($"Delete {targetPath}");
            Directory.Delete(targetPath, true);

            Debug.Log($"Update Form: {sourcePath} \n\t\t To: {targetPath}");
            DirectoryUtility.CopyDirectory(sourcePath, targetPath);
            Debug.Log($"Update Assets End");
        }
    }
}