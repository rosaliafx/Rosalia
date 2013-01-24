namespace Rosalia.Core.FileSystem
{
    using System.IO;

    public interface IFile : IFileSystemItem
    {
        Stream ReadStream { get; }

        Stream WriteStream { get; }

        long Length { get; }

        string Name { get; }

        IDirectory Directory { get; }

        void CopyTo(IFile destination);

        void CopyTo(IDirectory directory);
    }
}