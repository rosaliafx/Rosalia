namespace Rosalia.TaskLib.Compression
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ICSharpCode.SharpZipLib.Zip;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;

    public class CompressTask : AbstractTask, IEnumerable
    {
        private readonly IList<FileToCompress> _sourceFiles = new List<FileToCompress>();

        public CompressTask(IFile destination)
        {
            Destination = destination;
        }

        public IFile Destination { get; private set; }

        public void Add(string entityPath, IFile file)
        {
            _sourceFiles.Add(new FileToCompress(file, entityPath));
        }

        public IEnumerator GetEnumerator()
        {
            return _sourceFiles.GetEnumerator();
        }

        protected override ITaskResult<Nothing> SafeExecute(TaskContext context)
        {
            if (Destination == null)
            {
                throw new Exception("Destination file is not set");
            }

            using (var zipStream = new ZipOutputStream(Destination.WriteStream))
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

            return Success;
        }
    }
}