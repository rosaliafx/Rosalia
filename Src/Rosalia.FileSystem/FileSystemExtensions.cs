namespace Rosalia.FileSystem
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A set of helpful extension methods for file system objects
    /// </summary>
    public static class FileSystemExtensions
    {
        /// <summary>
        /// Writes a string to a file.
        /// </summary>
        /// <param name="content">string content</param>
        /// <param name="file">destination file</param>
        public static void WriteStringToFile(this IFile file, string content)
        {
            using (var writer = new StreamWriter(file.WriteStream))
            {
                writer.Write(content);
            }
        }

        public static string ReadAllText(this IFile file)
        {
            using (var reader = new StreamReader(file.ReadStream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Retrieves all directory files recursively.
        /// </summary>
        /// <param name="directory">target directory</param>
        /// <returns>a list of files</returns>
        public static FileList SearchFilesIn(this IDirectory directory)
        {
            return new FileList(GetFilesRecursivelySource(directory), directory);
        }

        private static IEnumerable<IFile> GetFilesRecursivelySource(this IDirectory directory)
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