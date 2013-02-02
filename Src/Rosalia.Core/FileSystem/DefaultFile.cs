namespace Rosalia.Core.FileSystem
{
    using System;
    using System.Diagnostics;
    using System.IO;

    [DebuggerDisplay("{AbsolutePath}")]
    public class DefaultFile : FileSystemItem, IFile
    {
        public DefaultFile(string absolutePath)
            : base(absolutePath)
        {
        }

        public override bool Exists
        {
            get
            {
                return File.Exists(AbsolutePath);
            }
        }

        public string NameWithoutExtension
        {
            get { return Path.GetFileNameWithoutExtension(AbsolutePath); }
        }

        public Stream ReadStream
        {
            get { return File.Open(AbsolutePath, FileMode.OpenOrCreate); }
        }

        public Stream WriteStream
        {
            get { return File.Open(AbsolutePath, FileMode.Create); }
        }

        public ExtensionInfo Extension
        {
            get { return new ExtensionInfo(Path.GetExtension(AbsolutePath)); }
        }

        public long Length
        {
            get { return new FileInfo(AbsolutePath).Length; }
        }

        public IDirectory Directory
        {
            get { return new DefaultDirectory(Path.GetDirectoryName(AbsolutePath)); }
        }

        public override string Name
        {
            get { return Path.GetFileName(AbsolutePath); }
        }

        public void CopyTo(IFile destination)
        {
            if (!Exists)
            {
                throw new Exception(string.Format("File {0} does not exist!", AbsolutePath));
            }

            File.Copy(AbsolutePath, destination.AbsolutePath, true);
        }

        public void CopyTo(IDirectory directory)
        {
            CopyTo(directory.GetFile(Name));
        }

        public override void EnsureExists()
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

        public override void Delete()
        {
            if (Exists)
            {
                File.Delete(AbsolutePath);    
            }
        }
    }
}