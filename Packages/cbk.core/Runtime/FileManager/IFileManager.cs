using System.Threading.Tasks;

namespace CBK.Core
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// 写入文件(不存在则创建)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void WriteFile(string filePath, string strData = default, bool bAppend = false);

        /// <summary>
        /// 异步写入文件(不存在则创建)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="strData"></param>
        /// <param name="bAppend"></param>
        /// <returns></returns>
        public Task WriteFileAsync(string filePath, string strData = default, bool bAppend = false);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void DeleteFile(string filePath);

        /// <summary>
        /// 检查文件存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CheckExist(string path);
    }
}