namespace Rosalia.Core.Tests.Stubs
{
    using System.Collections.Generic;
    using System.IO;
    using Rosalia.Core.FileSystem;

    public class DirectoryStub : IDirectory
    {
        public string AbsolutePath { get; set; }

        public bool Exists { get; set; }

        public IEnumerable<IDirectory> Directories { get; set; }

        public FileList Files { get; set; }

        public IDirectory Parent { get; set; }

        public void EnsureExists()
        {
        }

        public IDirectory GetRelative(string relativePath)
        {
            return new DirectoryStub
            {
                AbsolutePath = Path.Combine(AbsolutePath, relativePath)
            };
        }

        public IDirectory GetDirectory(string name)
        {
            return GetRelative(name);
        }

        public IFile GetFile(string name)
        {
            return new FileStub
            {
                AbsolutePath = Path.Combine(AbsolutePath, name)
            };
        }
    }
}