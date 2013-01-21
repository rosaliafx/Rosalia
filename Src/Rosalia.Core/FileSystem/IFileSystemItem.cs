namespace Rosalia.Core.FileSystem
{
    public interface IFileSystemItem
    {
        string AbsolutePath { get; }

        bool Exists { get; }

        void EnsureExists();
    }
}