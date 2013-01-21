namespace Rosalia.Core.FileSystem
{
    using System.Collections.Generic;

    public interface IDirectory : IFileSystemItem
    {
        IEnumerable<IDirectory> Directories { get; }

        IEnumerable<IFile> Files { get; }

        IDirectory Parent { get; }

        IDirectory GetRelative(string relativePath);

        IDirectory GetDirectory(string name);

        IFile GetFile(string name);
    }
}