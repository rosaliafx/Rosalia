namespace Rosalia.TestingSupport.FileSystem
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Rosalia.FileSystem;

    public class DirectoryStub : DefaultDirectory, IFileSystemStub, IEnumerable<IFileSystemStub>
    {
        private bool _exists;
        private IList<IFileSystemStub> _children;
        private readonly string _name;

        public DirectoryStub(string name, bool exists = true) : base(name)
        {
            _name = name;
            _exists = exists;
            Children = new List<IFileSystemStub>();
        }

        public DirectoryStub ParentDirectory { get; set; }

        public override string AbsolutePath
        {
            get
            {
                if (ParentDirectory == null)
                {
                    return _name;
                }

                return Path.Combine(ParentDirectory.AbsolutePath, _name);
            }
        }

        public void Add(IFileSystemStub child)
        {
            child.ParentDirectory = this;
            Children.Add(child);
        }

        public IList<IFileSystemStub> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                foreach (var item in _children)
                {
                    item.ParentDirectory = this;
                }
            }
        }

        public override bool Exists
        {
            get { return _exists; }
        }

        public override FileList Files
        {
            get
            {
                var files = Children.OfType<FileStub>().Where(file => file.Exists);

                return new FileList(files, this);
            }
        }

        public override IEnumerable<IDirectory> Directories
        {
            get
            {
                return Children.OfType<DirectoryStub>().Where(dir => dir.Exists);
            }
        }

        public override IDirectory Parent
        {
            get
            {
                return ParentDirectory;
            }
        }

        public override IDirectory GetDirectory(string name)
        {
            var existingDirectory = Children.OfType<DirectoryStub>().FirstOrDefault(dir => dir.Name == name);
            var directoryStub = existingDirectory ?? new DirectoryStub(name, false)
            {
                ParentDirectory = this
            };

            if (existingDirectory != null)
            {
                Children.Add(directoryStub);    
            }
            
            return directoryStub;
        }

        public override IFile GetFile(string name)
        {
            var existing = Children.OfType<FileStub>().FirstOrDefault(dir => dir.Name == name);
            var result = existing ?? new FileStub(name, false)
            {
                ParentDirectory = this
            };

            if (existing != null)
            {
                Children.Add(result);
            }

            return result;
        }

        public override void EnsureExists()
        {
            _exists = true;
        }

        public override void Delete()
        {
            _exists = false;
        }

        public IEnumerator<IFileSystemStub> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}