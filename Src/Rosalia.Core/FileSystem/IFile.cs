namespace Rosalia.Core.FileSystem
{
    using System.IO;

    public interface IFile : IFileSystemItem
    {
        string NameWithoutExtension { get; }

        Stream ReadStream { get; }

        Stream WriteStream { get; }

        ExtensionInfo Extension { get; }

        long Length { get; }

        IDirectory Directory { get; }

        void CopyTo(IFile destination);

        void CopyTo(IDirectory directory);

        
    }
}