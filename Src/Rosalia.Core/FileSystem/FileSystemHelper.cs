namespace Rosalia.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Simple helper class that provides methods for common File System usage cases.
    /// </summary>
    public class FileSystemHelper
    {
        public string GetFileName(IFile file)
        {
            return Path.GetFileName(file.AbsolutePath);
        }

        /// <summary>
        /// Writes a string to a file.
        /// </summary>
        /// <param name="content">string content</param>
        /// <param name="file">destination file</param>
        public void WriteStringToFile(string content, IFile file)
        {
            using (var writer = new StreamWriter(file.WriteStream))
            {
                writer.Write(content);
            }
        }

        /// <summary>
        /// Retrieves all directory files recursively.
        /// </summary>
        /// <param name="directory">target directory</param>
        /// <returns>a list of files</returns>
        public FileList GetFilesRecursively(IDirectory directory)
        {
            return new FileList(GetFilesRecursivelySource(directory));
        }

        public void CopyFilesToDirectory(FileList files, IDirectory destination)
        {
            CopyFilesToDirectory(files.All, destination);
        }

        public void CopyFilesToDirectory(IEnumerable<IFile> files, IDirectory destination)
        {
            destination.EnsureExists();
            foreach (var file in files)
            {
                var fileName = GetFileName(file);
                file.CopyTo(destination.GetFile(fileName));
            }
        }

        private IEnumerable<IFile> GetFilesRecursivelySource(IDirectory directory)
        {
            if (directory.Files != null)
            {
                foreach (var file in directory.Files)
                {
                    yield return file;
                }
            }

            if (directory.Directories != null)
            {
                foreach (var childDirectory in directory.Directories)
                {
                    foreach (var childFile in GetFilesRecursivelySource(childDirectory))
                    {
                        yield return childFile;
                    }
                }
            }
        }
    }
}