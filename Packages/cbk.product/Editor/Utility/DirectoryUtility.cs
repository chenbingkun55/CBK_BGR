using System.IO;
using System.Collections.Generic;

namespace CBK.Editor.Utility
{
    public static class DirectoryUtility
    {
        public static void CopyDirectory(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                File.Copy(file, Path.Combine(destinationPath, Path.GetFileName(file)), true);
            }

            foreach (var directory in Directory.GetDirectories(sourcePath))
            {
                var newDestination = Path.Combine(destinationPath, Path.GetFileName(directory));
                CopyDirectory(directory, newDestination);
            }
        }
    }
}