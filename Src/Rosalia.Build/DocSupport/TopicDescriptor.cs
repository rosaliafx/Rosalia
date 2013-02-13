namespace Rosalia.Build.DocSupport
{
    using Rosalia.Core.FileSystem;

    public class TopicDescriptor
    {
        public TopicDescriptor(IFile contentFile, string[] path)
        {
            ContentFile = contentFile;
            Path = path;
        }

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