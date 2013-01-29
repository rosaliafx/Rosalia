namespace Rosalia.Build.Tasks
{
    using System;
    using Rosalia.Core.FileSystem;

    public class TopicItem
    {
        public TopicItem(IDirectory docs, IFile contentFile)
        {
            Docs = docs;
            ContentFile = contentFile;

            var pathString = contentFile.Directory.AbsolutePath.ToLower().Replace(docs.AbsolutePath.ToLower(), string.Empty);
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