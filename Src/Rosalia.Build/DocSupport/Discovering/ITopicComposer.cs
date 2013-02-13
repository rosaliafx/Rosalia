namespace Rosalia.Build.DocSupport.Discovering
{
    using System.Collections.Generic;
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    public interface ITopicComposer
    {
        IEnumerable<ComposedTopic> Compose(
            IEnumerable<TopicDescriptor> descriptors,
            IDirectory partialDirectory);
    }
}