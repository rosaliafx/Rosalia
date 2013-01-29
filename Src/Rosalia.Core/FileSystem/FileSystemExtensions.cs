namespace Rosalia.Core.FileSystem
{
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
    }
}