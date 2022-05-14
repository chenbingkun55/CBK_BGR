using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CBK.Core
{
    public class FileManager : IFileManager
    {
        bool IFileManager.WriteFile(string filePath, string strData, bool bAppend)
        {
            return FileManager.WriteFile(filePath, strData, bAppend);
        }
        public static bool WriteFile(string filePath, string strData = default, bool bAppend = false)
        {
            var fullPath = Path.Combine(Config.kDataPath, filePath);
            var dirPath = fullPath.Substring(0, fullPath.LastIndexOf(Path.DirectorySeparatorChar));
            
            // 不存在则创建
            if(!CheckExist(fullPath))
            {
                Directory.CreateDirectory(dirPath);
                File.WriteAllText(fullPath, "");
            }

            try
            {
                using StreamWriter file = new StreamWriter(fullPath, append: bAppend);
                file.WriteLine(strData);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }

        async Task IFileManager.WriteFileAsync(string filePath, string strData, bool bAppend)
        {
            await FileManager.WriteFileAsync(filePath, strData, bAppend);
        }

        public static async Task WriteFileAsync(string filePath, string strData = default, bool bAppend = false)
        {
            var fullPath = Path.Combine(Config.kDataPath, filePath);
            var dirPath = fullPath.Substring(0, fullPath.LastIndexOf(Path.DirectorySeparatorChar));
            
            // 不存在则创建
            if(!CheckExist(fullPath))
            {
                Directory.CreateDirectory(dirPath);
                File.WriteAllText(fullPath, "");
            }

            try
            {
                using StreamWriter file = new StreamWriter(fullPath, append: bAppend);
                await file.WriteLineAsync(strData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to async write to {fullPath} with exception {e}");
            }
        }

        bool IFileManager.ReadFile(out List<string> outLines, string filePath)
        {
            return FileManager.ReadFile(out outLines, filePath);
        }
        public static bool ReadFile(out List<string> outLines, string filePath)
        {
            outLines = new List<string>();
            var fullPath = Path.Combine(Config.kDataPath, filePath);
            var dirPath = fullPath.Substring(0, fullPath.LastIndexOf(Path.DirectorySeparatorChar));

            if(!CheckExist(fullPath))
            {
                Directory.CreateDirectory(dirPath);
                File.WriteAllText(fullPath, "");
            }
            
            try
            {
                foreach (string line in File.ReadLines(fullPath))
                {  
                    outLines.Add(line);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read lines from {fullPath} with exception {e}");
                return false;
            }
        }

        bool IFileManager.ReadFile(out string outString, string filePath)
        {
            return FileManager.ReadFile(out outString, filePath);
        }
        public static bool ReadFile(out string outString, string filePath)
        {
            var fullPath = Path.Combine(Config.kDataPath, filePath);
            var dirPath = fullPath.Substring(0, fullPath.LastIndexOf(Path.DirectorySeparatorChar));
            
            if(!CheckExist(fullPath))
            {
                Directory.CreateDirectory(dirPath);
                File.WriteAllText(fullPath, "");
            }
            
            try
            {
                outString = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read line from {fullPath} with exception {e}");
                outString = "";
                return false;
            }
        }

        void IFileManager.DeleteFile(string filePath)
        {
            FileManager.DeleteFile(filePath);
        }
        public static void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(Config.kDataPath, filePath);

            try
            {
                File.Delete(fullPath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to delete {fullPath} with exception {e}");
            }
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