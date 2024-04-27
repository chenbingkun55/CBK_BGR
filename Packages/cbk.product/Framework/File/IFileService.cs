using System.IO;

namespace CBK.Framework.File
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 指定路径文件是否存在
        /// </summary>
        bool Exists(string path);

        /// <summary>
        /// 指定路径文件夹是否存在
        /// </summary>
        bool ExistsDirectory(string path);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        bool CreateDirectory(string path);

        /// <summary>
        /// 读取指定文件
        /// </summary>
        Stream OpenRead(string path);
        
        /// <summary>
        /// 写入指定文件
        /// </summary>
        Stream OpenWrite(string path);
    }
}