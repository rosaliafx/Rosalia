namespace Rosalia.Core.FileSystem
{
    using System;
    using System.IO;

    public class DefaultFile : IFile
    {
        public DefaultFile(string absolutePath)
        {
            AbsolutePath = absolutePath;
        }

        public string AbsolutePath { get; private set; }

        public bool Exists
        {
            get
            {
                return File.Exists(AbsolutePath);
            }
        }

        public Stream ReadStream
        {
            get { return File.Open(AbsolutePath, FileMode.OpenOrCreate); }
        }

        public Stream WriteStream
        {
            get { return File.Open(AbsolutePath, FileMode.Create); }
        }

        public long Length
        {
            get { return new FileInfo(AbsolutePath).Length; }
        }

        public IDirectory Directory
        {
            get { return new DefaultDirectory(Path.GetDirectoryName(AbsolutePath)); }
        }

        public string Name
        {
            get { return Path.GetFileName(AbsolutePath); }
        }

        public void CopyTo(IFile destination)
        {
            if (!Exists)
            {
                throw new Exception(string.Format("File {0} does not exist!", AbsolutePath));
            }

            File.Copy(AbsolutePath, destination.AbsolutePath);
        }

        public void CopyTo(IDirectory directory)
        {
            CopyTo(directory.GetFile(Name));
        }

        public void EnsureExists()
        {
            if (Exists)
            {
                return;
            }

            Directory.EnsureExists();
            using (File.Create(AbsolutePath))
            {
            }
        }
    }
}