namespace Rosalia.FileSystem
{
    using System.Collections.Generic;

    public abstract class IDirectory : FileSystemItem
    {
        protected IDirectory(string absolutePath): base(absolutePath)
        {
        }
        
        public abstract IEnumerable<IDirectory> Directories { get; }

        public abstract FileList Files { get; }

        public abstract IDirectory Parent { get; }

        public abstract IDirectory GetDirectory(string name);

        public abstract IFile GetFile(string name);

        public abstract UnknownFileSystemItem this[string path] { get; }
    }
}