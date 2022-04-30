using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CBK.Core
{
    public class FileManager : IFileManager
    {
        void IFileManager.WriteFile(string filePath, string strData, bool bAppend)
        {
            FileManager.WriteFile(filePath, strData, bAppend);
        }

        public static void WriteFile(string filePath, string strData = default, bool bAppend = false)
        {
            var realFilePath = Path.Combine(Config.kDataPath, filePath);
            var dirPath = realFilePath.Substring(0, realFilePath.LastIndexOf('/'));
            
            // 目录不存在则创建
            if(!CheckExist(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using StreamWriter file = new StreamWriter(realFilePath, append: bAppend);
            file.WriteLine(strData);
        }

        async Task IFileManager.WriteFileAsync(string filePath, string strData, bool bAppend)
        {
            await FileManager.WriteFileAsync(filePath, strData, bAppend);
        }

        public static async Task WriteFileAsync(string filePath, string strData = default, bool bAppend = false)
        {
            var realFilePath = Path.Combine(Config.kDataPath, filePath);
            var dirPath = realFilePath.Substring(0, realFilePath.LastIndexOf('/'));
            
            // 目录不存在则创建
            if(!CheckExist(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using StreamWriter file = new StreamWriter(realFilePath, append: bAppend);
            await file.WriteLineAsync(strData);
        }

        void IFileManager.DeleteFile(string filePath)
        {
            FileManager.DeleteFile(filePath);
        }
        public static void DeleteFile(string filePath)
        {
            var raleFilePath = Path.Combine(Config.kDataPath, filePath);
            File.Delete(raleFilePath);
        }

        bool IFileManager.CheckExist(string filePath)
        {
            return FileManager.CheckExist(filePath);
        }
        public static bool CheckExist(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}