namespace Rosalia.Build.Tasks
{
    using System;
    using Rosalia.Core.FileSystem;

    public class TopicItem
    {
        public TopicItem(IDirectory docs, IFile contentSource)
        {
            Docs = docs;
            ContentFile = contentSource;

            var pathString = contentSource.Directory.AbsolutePath.Equals(docs.AbsolutePath, StringComparison.InvariantCultureIgnoreCase) ?
                string.Empty :
                contentSource.Directory.GetRelativePath(docs).ToLower();

            Path = pathString.Split(new[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        public IDirectory Docs { get; private set; }

        public IFile ContentFile { get; private set; }

        public string[] Path { get; private set; }

        public int Level
        {
            get { return Path.Length; }
        }

        public string Key
        {
            get { return ContentFile.NameWithoutExtension; }
        }
    }
}