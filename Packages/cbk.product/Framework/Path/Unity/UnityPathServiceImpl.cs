using System;
using CBK.Framework.Core;
using UnityEngine;

namespace CBK.Framework.Path.Unity
{
    /// <summary>
    /// 基于Unity的路径服务实现
    /// </summary>
    internal sealed class UnityPathServiceImpl : IInitialize, IPathService
    {
        public void Initialize()
        {
            _dataPath = Application.dataPath;
            _streamingAssetPath = Application.streamingAssetsPath;
            _persistentDataPath = Application.persistentDataPath;
        }

        public string Resolve(PathType pathType, string path)
        {
            return pathType switch
            {
                PathType.DataPath => System.IO.Path.Combine(_dataPath, path),
                PathType.StreamingAssetsPath => System.IO.Path.Combine(_streamingAssetPath, path),
                PathType.PersistentDataPath => System.IO.Path.Combine(_persistentDataPath, path),
                _ => throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null)
            };
        }

        public string Resolve(PathType pathType, string path1, string path2)
        {
            return pathType switch
            {
                PathType.DataPath => System.IO.Path.Combine(_dataPath, path1, path2),
                PathType.StreamingAssetsPath => System.IO.Path.Combine(_streamingAssetPath, path1, path2),
                PathType.PersistentDataPath => System.IO.Path.Combine(_persistentDataPath, path1, path2),
                _ => throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null)
            };
        }

        public string Resolve(PathType pathType, string path1, string path2, string path3)
        {
            return pathType switch
            {
                PathType.DataPath => System.IO.Path.Combine(_dataPath, path1, path2, path3),
                PathType.StreamingAssetsPath => System.IO.Path.Combine(_streamingAssetPath, path1, path2, path3),
                PathType.PersistentDataPath => System.IO.Path.Combine(_persistentDataPath, path1, path2, path3),
                _ => throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null)
            };
        }

        public string Resolve(PathType pathType, params string[] paths)
        {
            var resize = new string[paths.Length + 1];
            Array.Copy(paths, 0, resize, 1, paths.Length);

            resize[0] = pathType switch
            {
                PathType.DataPath => _dataPath,
                PathType.StreamingAssetsPath => _streamingAssetPath,
                PathType.PersistentDataPath => _persistentDataPath,
                _ => throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null)
            };

            return System.IO.Path.Combine(resize);
        }

        private string _dataPath;
        private string _streamingAssetPath;
        private string _persistentDataPath;
    }
}