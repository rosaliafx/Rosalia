namespace Rosalia.Core.FileSystem
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    [DebuggerDisplay("{AbsolutePath}")]
    public class DefaultDirectory : FileSystemItem, IDirectory
    {
        public DefaultDirectory(string absolutePath) : base(absolutePath)
        {
        }

        public override bool Exists
        {
            get { return Directory.Exists(AbsolutePath); }
        }

        public override string Name
        {
            get { return Path.GetFileName(AbsolutePath); }
        }

        public FileList Files
        {
            get
            {
                var files = Directory.GetFiles(AbsolutePath).Select(file => new DefaultFile(file));

                return new FileList(files, this);
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

        public override void EnsureExists()
        {
            if (Exists)
            {
                return;
            }

            Directory.CreateDirectory(AbsolutePath);
        }
    }
}