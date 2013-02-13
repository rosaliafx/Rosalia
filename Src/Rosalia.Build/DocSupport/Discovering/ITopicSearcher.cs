namespace Rosalia.Build.DocSupport.Discovering
{
    using System.Collections.Generic;
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    public interface ITopicSearcher
    {
        IEnumerable<TopicDescriptor> Find(IEnumerable<IDirectory> directories);
    }
}