namespace Rosalia.Core.FileSystem
{
    using System.IO;

    public interface IFile : IFileSystemItem
    {
        Stream ReadStream { get; }

        Stream WriteStream { get; }

        long Length { get; }

        IDirectory Directory { get; }

        string Name { get; }

        void CopyTo(IFile destination);
    }
}