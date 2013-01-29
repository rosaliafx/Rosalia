namespace Rosalia.Build.DocSupport
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    [DebuggerDisplay("{Key} - {Title}")]
    public class TableOfContentsItem
    {
        public TableOfContentsItem()
        {
            Children = new List<TableOfContentsItem>();
        }

        public TopicItem TopicItem { get; set; }

        public string Key { get; set; }

        public string Title { get; set; }

        public IList<TableOfContentsItem> Children { get; set; }

        public IFile ContentFile { get; set; }
    }
}