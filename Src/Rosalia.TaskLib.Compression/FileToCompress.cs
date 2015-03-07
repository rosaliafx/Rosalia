namespace Rosalia.TaskLib.Compression
{
    using Rosalia.FileSystem;

    public class FileToCompress
    {
        public FileToCompress(IFile file, string entityPath)
        {
            File = file;
            EntityPath = entityPath;
        }

        public IFile File { get; private set; }

        public string EntityPath { get; private set; }
    }
}