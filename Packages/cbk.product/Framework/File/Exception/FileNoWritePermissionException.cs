namespace CBK.Framework.File.Exception
{
    /// <summary>
    /// 文件没有写入权限异常
    /// </summary>
    public sealed class FileNoWritePermissionException : System.Exception
    {
        public string filePath { get; }

        public FileNoWritePermissionException(string filePath)
        {
            this.filePath = filePath;
        }
    }
}