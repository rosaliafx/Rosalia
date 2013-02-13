namespace Rosalia.Build.DocSupport
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    [DebuggerDisplay("{Key} - {Title}")]
    public class ComposedTopic
    {
        public ComposedTopic()
        {
            Children = new List<ComposedTopic>();
        }

        public TopicDescriptor Descriptor { get; set; }

        public string Key
        {
            get { return Descriptor.Key; }
        }

        public string Title { get; set; }

        public IList<ComposedTopic> Children { get; set; }

        public IFile FormattedContentFile { get; set; }

        public string RelativeResultFilePath
        {
            get { return Path.Combine(string.Join("/", Descriptor.Path), Descriptor.Key) + ".html"; }
        }
    }
}