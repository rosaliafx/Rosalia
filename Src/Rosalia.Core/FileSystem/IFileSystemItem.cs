namespace Rosalia.Core.FileSystem
{
    public interface IFileSystemItem
    {
        string AbsolutePath { get; }

        bool Exists { get; }

        string Name { get; }

        void EnsureExists();
    }
}