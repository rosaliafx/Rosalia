namespace Rosalia.FileSystem
{
    public class UnknownFileSystemItem
    {
        private readonly IDirectory _parent;
        private readonly string _name;

        public UnknownFileSystemItem(IDirectory parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public UnknownFileSystemItem this[string path]
        {
            get
            {
                return new UnknownFileSystemItem(this.AsDirectory(), path);
            }
        }

        public IFile AsFile()
        {
            return _parent.GetFile(_name);
        }

        public IDirectory AsDirectory()
        {
            return _parent.GetDirectory(_name);
        }

        public static implicit operator IDirectory(UnknownFileSystemItem item)
        {
            return item.AsDirectory();
        }

        public static implicit operator IFile(UnknownFileSystemItem item)
        {
            return item.AsFile();
        }
    }
}