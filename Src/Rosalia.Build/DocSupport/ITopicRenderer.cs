namespace Rosalia.Build.DocSupport
{
    using System.Collections.Generic;
    using Rosalia.Core.FileSystem;

    public interface ITopicRenderer
    {
        void Render(
            ComposedTopic topic, 
            IEnumerable<ComposedTopic> allTopics,
            IFile output,
            IDirectory hostDirectory,
            IDirectory stylesDirectory,
            IDirectory scriptsDirectory);
    }
}