namespace Rosalia.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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

        [Obsolete("Use SearchFilesRecursively")]
        public static FileList SearchFilesIn(this IDirectory directory)
        {
            return new FileList(GetFilesRecursivelySource(directory), directory);
        }

        /// <summary>
        /// Retrieves all directory files recursively.
        /// </summary>
        /// <param name="directory">target directory</param>
        /// <returns>a list of files</returns>
        public static FileList SearchFilesRecursively(this IDirectory directory)
        {
            return new FileList(GetFilesRecursivelySource(directory), directory);
        }

        /// <summary>
        /// Finds closest directory matching the predicate. 
        /// Checks the directory itself and it's parents.
        /// Returns null if none of parent directories match the predicate.
        /// </summary>
        public static IDirectory Closest(this IDirectory directory, Predicate<IDirectory> predicate)
        {
            return directory.GetParentsChain().FirstOrDefault(dir => predicate(dir));
        }

        /// <summary>
        /// Yields directory itself and all it's parents.
        /// </summary>
        public static IEnumerable<IDirectory> GetParentsChain(this IDirectory directory)
        {
            IDirectory currentDirectory = directory;
            while (currentDirectory != null)
            {
                yield return currentDirectory;
                currentDirectory = currentDirectory.Parent;
            }
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