namespace Rosalia.Core.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Simple file list wrapper that allows to perform typical sorting and filtering operations.
    /// </summary>
    public class FileList
    {
        private readonly IEnumerable<IFile> _source;

        public FileList(IEnumerable<IFile> source)
        {
            _source = source;
        }

        /// <summary>
        /// Gets all files in the list.
        /// </summary>
        public IEnumerable<IFile> All
        {
            get { return _source; }
        }

        public FileList Filter(Predicate<IFile> predicate)
        {
            return new FileList(All.Where(f => predicate(f)));
        }

        public FileList Exclude(Predicate<IFile> predicate)
        {
            return Filter(file => !predicate(file));
        }

        public FileList Filter(Predicate<string> predicate)
        {
            return Filter(file => predicate(file.AbsolutePath));
        }

        public FileList Exclude(Predicate<string> predicate)
        {
            return Exclude(file => predicate(file.AbsolutePath));
        }

        public FileList Filter(Regex regex)
        {
            return Filter(file => regex.IsMatch(file.AbsolutePath));
        }

        public FileList Exclude(Regex regex)
        {
            return Exclude(file => regex.IsMatch(file.AbsolutePath));
        }

        public FileList FilterWithRegex(string pattern)
        {
            var regex = new Regex(pattern);
            return Filter(regex);
        }

        public FileList ExcludeWithRegex(string pattern)
        {
            var regex = new Regex(pattern);
            return Exclude(regex);
        }
    }
}