using CLAP;

namespace Tests
{
    internal class FileSystemMock : FileSystemHelper.IFileSystem
    {
        public string ReturnValue { get; set; }

        public string ReadAllText(string path)
        {
            return ReturnValue;
        }
    }
}