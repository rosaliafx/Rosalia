namespace Rosalia.Core.Tests.Stubs
{
    using System.IO;
    using System.Text;
    using Rosalia.Core.FileSystem;

    public class FileStub : IFile
    {
        private MemoryStream _contentStream;

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

                _contentStream = new MemoryStream(Encoding.Default.GetBytes(value));
            }
        }

        public string AbsolutePath { get; set; }

        public bool Exists { get; set; }

        public string NameWithoutExtension { get; set; }

        public Stream ReadStream
        {
            get { return _contentStream; }
        }

        public Stream WriteStream
        {
            get
            {
                _contentStream = new MemoryStream();
                return _contentStream;
            }
        }

        public ExtensionInfo Extension { get; set; }

        public long Length
        {
            get { return _contentStream.Length; }
        }

        public IDirectory Directory { get; set; }

        public string Name { get; private set; }

        public void CopyTo(IFile destination)
        {
        }

        public void CopyTo(IDirectory directory)
        {
        }

        public void EnsureExists()
        {
        }

        public string GetRelativePath(IDirectory directory)
        {
            throw new System.NotImplementedException();
        }
    }
}