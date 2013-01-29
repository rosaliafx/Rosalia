namespace Rosalia.Build.DocSupport
{
    using System.Collections.Generic;
    using Rosalia.Core.FileSystem;

    public interface ITopicRenderer
    {
        void Render(
            TableOfContentsItem topic, 
            IEnumerable<TableOfContentsItem> allTopics,
            IFile output, 
            IDirectory hostDirectory);
    }
}