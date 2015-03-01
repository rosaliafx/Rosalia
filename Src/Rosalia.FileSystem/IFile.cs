namespace Rosalia.FileSystem
{
    using System.IO;

    public abstract class IFile : FileSystemItem
    {
        protected IFile(string absolutePath) : base(absolutePath)
        {
        }

        public abstract string NameWithoutExtension { get; }

        public abstract Stream ReadStream { get; }

        public abstract Stream WriteStream { get; }

        public abstract ExtensionInfo Extension { get; }

        public abstract long Length { get; }

        public abstract IDirectory Directory { get; }

        public abstract void CopyTo(IFile destination);

        public abstract void CopyTo(IDirectory directory);
    }
}