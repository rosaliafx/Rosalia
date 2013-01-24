namespace Rosalia.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Simple helper class that provides methods for common File System usage cases.
    /// </summary>
    public class FileSystemHelper
    {
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
        public FileList SearchFilesIn(IDirectory directory)
        {
            return new FileList(GetFilesRecursivelySource(directory), directory);
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