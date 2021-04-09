using System;
using System.IO;

namespace AssemblyChanges
{
    public static class Utils
    {
        public static void Validation(string assemblyFile, ref string destinationPath)
        {
            if (!File.Exists(assemblyFile))
            {
                throw new FileNotFoundException("File are not found!");
            }

            if (Path.GetExtension(assemblyFile).ToLower() != ".dll" &&
                Path.GetExtension(assemblyFile).ToLower() != ".exe")
            {
                throw new ArgumentException("Assembly file is not C# build!");
            }

            if (!File.GetAttributes(destinationPath).HasFlag(FileAttributes.Directory))
            {
                destinationPath = Path.GetDirectoryName(destinationPath);
            }

            destinationPath = Path.Combine(destinationPath!, "changedBuild");

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }
        }

        public static void CopyAssembly(string assemblyFile, string destinationDirectory)
        {
            var assemblyDirectory = Path.GetDirectoryName(assemblyFile) ?? string.Empty;

            foreach (var dirPath in Directory.GetDirectories(assemblyDirectory, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(assemblyDirectory, destinationDirectory));
            }

            foreach (var newPath in Directory.GetFiles(assemblyDirectory, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(assemblyDirectory, destinationDirectory), true);
            }
        }
    }
}