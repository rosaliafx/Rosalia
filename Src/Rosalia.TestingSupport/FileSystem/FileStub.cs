namespace Rosalia.TestingSupport.FileSystem
{
    using System.IO;
    using System.Text;
    using Rosalia.FileSystem;

    public class FileStub : DefaultFile, IFileSystemStub
    {
        private readonly string _name;
        private bool _exists;
        private MemoryStream _contentStream;

        public FileStub(string name, bool exists = true)
            : base(null)
        {
            _name = name;
            _exists = exists;
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

        public override bool Exists
        {
            get { return _exists; }
        }

        public string Content
        {
            get
            {
                if (_contentStream == null)
                {
                    return null;
                }

                return Encoding.Default.GetString(_contentStream.ToArray());
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _contentStream = null;
                    return;
                }

                _exists = true;
                _contentStream = new MemoryStream(Encoding.Default.GetBytes(value));
            }
        }
//
//        public string AbsolutePath { get; set; }
//
//        public override bool Exists { get; private set; }
//
//        public string NameWithoutExtension { get; set; }
//
        public override Stream ReadStream
        {
            get { return _contentStream; }
        }

        public override Stream WriteStream
        {
            get
            {
                _contentStream = new MemoryStream();
                _exists = true;
                return _contentStream;
            }
        }
//
//        public ExtensionInfo Extension { get; set; }
//
        public override long Length
        {
            get { return _contentStream.Length; }
        }
//
        public override IDirectory Directory
        {
            get { return ParentDirectory; }
        }

        
        public override void CopyTo(IFile destination)
        {
            ((FileStub) destination).Content = Content;
        }

        public void CopyTo(IDirectory directory)
        {
            // todo
        }

        public void EnsureExists()
        {
            _exists = true;
        }

        public void Delete()
        {
            _exists = false;
        }
        
    }
}