namespace Rosalia.Core.FileSystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Simple file list wrapper that allows to perform typical sorting and filtering operations.
    /// </summary>
    public class FileList : IEnumerable<IFile>
    {
        private readonly IDirectory _baseDirectory;
        private readonly IEnumerable<IFile> _source;

        public FileList(IEnumerable<IFile> source, IDirectory baseDirectory)
        {
            _source = source;
            _baseDirectory = baseDirectory;
        }

        /// <summary>
        /// Gets all files in the list.
        /// </summary>
        public IEnumerable<IFile> All
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets a base directory for the list of files.
        /// </summary>
        public IDirectory BaseDirectory
        {
            get { return _baseDirectory; }
        }

        public FileList IncludeByRelativePath(Predicate<string> predicate)
        {
            return Include(file => predicate(file.GetRelativePath(_baseDirectory)));
        }

        public FileList Include(Predicate<IFile> predicate)
        {
            return new FileList(All.Where(f => predicate(f)), BaseDirectory);
        }

        public FileList Exclude(Predicate<IFile> predicate)
        {
            return Include(file => !predicate(file));
        }

        public FileList Include(Predicate<string> predicate)
        {
            return Include(file => predicate(file.AbsolutePath));
        }

        public FileList Exclude(Predicate<string> predicate)
        {
            return Exclude(file => predicate(file.AbsolutePath));
        }

        public FileList IncludeByExtension(params string[] extensions)
        {
            return Include(file => extensions.Any(extension => file.Extension.Is(extension)));
        }

        public FileList IncludeByFileName(params string[] fileNames)
        {
            return Include(file => fileNames.Any(fileName => fileName.Equals(file.Name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public FileList Include(Regex regex)
        {
            return Include(file => regex.IsMatch(file.AbsolutePath));
        }

        public FileList Exclude(Regex regex)
        {
            return Exclude(file => regex.IsMatch(file.AbsolutePath));
        }

        public FileList FilterWithRegex(string pattern)
        {
            var regex = new Regex(pattern);
            return Include(regex);
        }

        public FileList ExcludeWithRegex(string pattern)
        {
            var regex = new Regex(pattern);
            return Exclude(regex);
        }

        public IEnumerator<IFile> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyAllTo(IDirectory directory)
        {
            foreach (var file in _source)
            {
                file.CopyTo(directory);
            }
        }

        public void CopyRelativelyTo(IDirectory directory)
        {
            foreach (var file in All)
            {
                var relativePath = file.GetRelativePath(_baseDirectory);
                var relativeDestination = directory.GetFile(relativePath);
                relativeDestination.EnsureExists();

                file.CopyTo(relativeDestination);
            }
        }
    }
}