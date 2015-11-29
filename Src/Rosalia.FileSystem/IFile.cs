namespace Rosalia.FileSystem
{
    using System.IO;

    public abstract class IFile : FileSystemItem
    {
        protected IFile(string absolutePath) : base(absolutePath)
        {
        }

        /// <summary>
        /// Gets file name without extension.
        /// </summary>
        public abstract string NameWithoutExtension { get; }

        public abstract Stream ReadStream { get; }

        public abstract Stream WriteStream { get; }

        public abstract ExtensionInfo Extension { get; }

        /// <summary>
        /// Gets size of the file in bytes.
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// Gets parent directory.
        /// </summary>
        public abstract IDirectory Directory { get; }

        public abstract void CopyTo(IFile destination);

        public abstract void CopyTo(IDirectory directory);
    }
}