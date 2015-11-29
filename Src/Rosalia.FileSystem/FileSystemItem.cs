namespace Rosalia.FileSystem
{
    using System;
    using System.IO;

    public abstract class FileSystemItem
    {
        protected FileSystemItem(string absolutePath)
        {
            AbsolutePath = absolutePath;
        }

        public virtual string AbsolutePath { get; private set; }

        /// <summary>
        /// Gets the flag indicating whether file or directory exists.
        /// </summary>
        public abstract bool Exists { get; }

        public abstract string Name { get; }

        public abstract void EnsureExists();

        public abstract void Delete();

        public string GetRelativePath(IDirectory directory)
        {
            var basePath = directory.AbsolutePath;
            if (!basePath.EndsWith("\\"))
            {
                basePath += "\\";
            }

            var thisItem = new Uri(AbsolutePath);
            var baseItem = new Uri(basePath);

            return Uri.UnescapeDataString(
                baseItem.MakeRelativeUri(thisItem)
                    .ToString()
                    .Replace('/', Path.DirectorySeparatorChar));
        }

        public override string ToString()
        {
            /* do implicit conversion to string */
            return this;
        }

        public static implicit operator string(FileSystemItem item)
        {
            return item.AbsolutePath;
        }
    }
}