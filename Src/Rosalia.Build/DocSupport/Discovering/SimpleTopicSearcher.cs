namespace Rosalia.Build.DocSupport.Discovering
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    public class SimpleTopicSearcher : ITopicSearcher
    {
        public IEnumerable<TopicDescriptor> Find(IEnumerable<IDirectory> directories)
        {
            var topicItems = new List<TopicDescriptor>();

            foreach (var directory in directories)
            {
                var docsDirectory = directory.GetDirectory("Docs");
                if (docsDirectory.Exists)
                {
                    var topicFiles = docsDirectory.SearchFilesIn().Include(file => file.Name.EndsWith(".md"));

                    foreach (var file in topicFiles)
                    {
                        var pathString = file.Directory.AbsolutePath.Equals(docsDirectory.AbsolutePath, StringComparison.InvariantCultureIgnoreCase) ?
                            string.Empty :
                            file.Directory.GetRelativePath(docsDirectory).ToLower();

                        var path = pathString.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                        topicItems.Add(new TopicDescriptor(file, path));
                    }
                }
            }

            return topicItems;
        }
    }
}