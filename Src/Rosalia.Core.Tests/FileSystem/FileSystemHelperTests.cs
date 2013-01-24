namespace Rosalia.Core.Tests.FileSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Tests.Stubs;

    [TestFixture]
    public class FileSystemHelperTests
    {
        private FileSystemHelper _fileSystem;

        [SetUp]
        public void Init()
        {
            _fileSystem = new FileSystemHelper();
        }

        [Test]
        public void WriteStringToFile_ShouldChangeFileContent()
        {
            var file = new FileStub();

            _fileSystem.WriteStringToFile("test", file);

            Assert.That(file.Content, Is.EqualTo("test"));
        }

        [Test]
        public void GetFilesRecursively_FlatDirectory_ShouldReturnFiles()
        {
            var directory = new DirectoryStub();
            directory.Files = new FileList(
                new List<IFile>
                {
                    new FileStub { AbsolutePath = "file1" },
                    new FileStub { AbsolutePath = "file2" },
                    new FileStub { AbsolutePath = "file3" }
                },
                directory);

            var list = _fileSystem.SearchFilesIn(directory).All.ToList();

            Assert.That(list, Is.Not.Null);
            Assert.That(list.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetFilesRecursively_DirectoryWithSubdirectory_ShouldReturnFiles()
        {
            var childDirectory = new DirectoryStub();
            childDirectory.Files = new FileList(
                new List<IFile>
                {
                    new FileStub { AbsolutePath = "file4" }, 
                    new FileStub { AbsolutePath = "file5" }
                }, childDirectory);

            var directory = new DirectoryStub
            {
                Directories = new List<IDirectory>
                {
                    childDirectory                      
                }
            };

            directory.Files = new FileList(new List<IFile>
            {
                new FileStub { AbsolutePath = "file1" },
                new FileStub { AbsolutePath = "file2" },
                new FileStub { AbsolutePath = "file3" }
            }, directory);

            var list = _fileSystem.SearchFilesIn(directory).All.ToList();

            Assert.That(list, Is.Not.Null);
            Assert.That(list.Count, Is.EqualTo(5));
        }
    }
}