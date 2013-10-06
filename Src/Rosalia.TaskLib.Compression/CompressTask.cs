namespace Rosalia.TaskLib.Compression
{
    using System.Collections.Generic;
    using ICSharpCode.SharpZipLib.Zip;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Helpers;
    using Rosalia.Core.Tasks;

    public class CompressTask<T> : AbstractLeafTask<T>
    {
        private readonly IList<FileToCompress> _sourceFiles = new List<FileToCompress>();

        public IFile Destination { get; set; }

        public IEnumerable<FileToCompress> SourceFiles
        {
            get { return _sourceFiles; }
        }

        public CompressTask<T> WithFile(string entityPath, IFile file)
        {
            _sourceFiles.Add(new FileToCompress(file, entityPath));
            return this;
        }

        public CompressTask<T> ToFile(IFile destination)
        {
            Destination = destination;
            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<T> context)
        {
            var destination = Destination.EnsureNotNull("Destination file");

            using (var zipStream = new ZipOutputStream(destination.WriteStream))
            {
                foreach (var fileToCompress in _sourceFiles)
                {
                    var entry = new ZipEntry(fileToCompress.EntityPath)
                    {
                        Size = fileToCompress.File.Length
                    };

                    zipStream.PutNextEntry(entry);

                    using (var fileReadStream = fileToCompress.File.ReadStream)
                    {
                        fileReadStream.CopyTo(zipStream);
                    }

                    zipStream.CloseEntry();
                }

                zipStream.IsStreamOwner = true;
            }
        }
    }
}