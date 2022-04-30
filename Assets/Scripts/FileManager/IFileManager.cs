
namespace CBK
{
    public interface IFileManager
    {
        public bool CreateFile(string path);
        public bool RemoveFile(string path);

        public bool CheckFileExist(string filePath);
    }
}