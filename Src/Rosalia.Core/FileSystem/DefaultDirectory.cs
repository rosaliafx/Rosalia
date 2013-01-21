namespace Rosalia.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DefaultDirectory : IDirectory
    {
        public DefaultDirectory(string absolutePath)
        {
            AbsolutePath = absolutePath;
        }

        public string AbsolutePath { get; private set; }

        public bool Exists
        {
            get { return Directory.Exists(AbsolutePath); }
        }

        public IEnumerable<IFile> Files
        {
            get 
            {
                return Directory
                    .GetFiles(AbsolutePath)
                    .Select(file => new DefaultFile(file));
            }
        }

        public IEnumerable<IDirectory> Directories
        {
            get 
            {
                return Directory
                    .GetDirectories(AbsolutePath)
                    .Select(file => new DefaultDirectory(file));
            }
        }


        public IDirectory Parent
        {
            get
            {
                return new DefaultDirectory(Directory.GetParent(AbsolutePath).FullName);
            }
        }

        public IDirectory GetRelative(string relativePath)
        {
            return new DefaultDirectory(Path.Combine(AbsolutePath, relativePath));
        }

        public IDirectory GetDirectory(string name)
        {
            var path = Path.Combine(AbsolutePath, name);
            return new DefaultDirectory(path);
        }

        public IFile GetFile(string name)
        {
            var path = Path.Combine(AbsolutePath, name);
            return new DefaultFile(path);
        }

        public void EnsureExists()
        {
            if (Exists)
            {
                return;
            }

            Directory.CreateDirectory(AbsolutePath);
        }
    }
}