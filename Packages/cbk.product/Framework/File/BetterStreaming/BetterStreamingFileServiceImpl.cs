using System.IO;
using CBK.Framework.Core;
using CBK.Framework.File.Exception;
using UnityEngine;

namespace CBK.Framework.File.BetterStreaming
{
    /// <summary>
    /// 基于BetterStreamingAssets的文件服务实现
    /// </summary>
    internal sealed class BetterStreamingFileServiceImpl : IInitialize, IFileService
    {
        public void Initialize()
        {
            _streamingAssetsPath = Application.streamingAssetsPath;
            _subStringIndex = _streamingAssetsPath.Length + 1;

            BetterStreamingAssets.Initialize();
        }

        public bool Exists(string path)
        {
            return TryParsePathInStreamingAssets(path, out var relativePath) ? BetterStreamingAssets.FileExists(relativePath) : System.IO.File.Exists(path);
        }

        public bool ExistsDirectory(string path)
        {
            return TryParsePathInStreamingAssets(path, out var relativePath) ? BetterStreamingAssets.DirectoryExists(relativePath) : Directory.Exists(path);
        }

        public bool CreateDirectory(string path)
        {
            if (TryParsePathInStreamingAssets(path, out _))
                return false;
            
            if (System.IO.File.Exists(path))
                return true;

            Directory.CreateDirectory(path);
            
            return true;
        }

        public Stream OpenRead(string path)
        {
            return TryParsePathInStreamingAssets(path, out var relativePath) ? BetterStreamingAssets.OpenRead(relativePath) : System.IO.File.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            if (TryParsePathInStreamingAssets(path, out _))
                throw new FileNoWritePermissionException(path);
            
            return System.IO.File.OpenWrite(path);
        }
        
        private bool TryParsePathInStreamingAssets(string path, out string relativePath)
        {
            if (path.Length <= _subStringIndex || !path.StartsWith(_streamingAssetsPath))
            {
                relativePath = string.Empty;
                return false;
            }

            relativePath = path.Substring(_subStringIndex);
            return relativePath.Length > 0;
        }
        
        private string _streamingAssetsPath;
        private int _subStringIndex;
    }
}