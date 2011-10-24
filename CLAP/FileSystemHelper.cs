using System.IO;

namespace CLAP
{
    public static class FileSystemHelper
    {
        public static IFileSystem FileHandler { get; set; }

        static FileSystemHelper()
        {
            FileHandler = new FileSystem();
        }

        internal static string ReadAllText(string path)
        {
            return FileHandler.ReadAllText(path);
        }

        public interface IFileSystem
        {
            string ReadAllText(string path);
        }

        private class FileSystem : IFileSystem
        {
            public string ReadAllText(string path)
            {
                return File.ReadAllText(path);
            }
        }
    }
}