namespace Rosalia.TaskLib.Compression
{
    using System.Collections.Generic;
    using Rosalia.Core.FileSystem;

    public class CompressTaskInput
    {
        private readonly IList<FileToCompress> _sourceFiles = new List<FileToCompress>();

        public IFile Destination { get; set; }

        public IEnumerable<FileToCompress> SourceFiles
        {
            get { return _sourceFiles; }
        }

        public CompressTaskInput WithFile(string entityPath, IFile file)
        {
            _sourceFiles.Add(new FileToCompress(file, entityPath));
            return this;
        }

        public CompressTaskInput ToFile(IFile destination)
        {
            Destination = destination;
            return this;
        }
    }
}