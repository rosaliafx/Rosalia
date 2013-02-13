namespace Rosalia.Build.DocSupport.Discovering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HtmlAgilityPack;
    using Rosalia.Build.DocSupport.Formatting;
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    public class DefaultTopicComposer : ITopicComposer
    {
        private readonly IDocFormatter _formatter;

        public DefaultTopicComposer(IDocFormatter formatter)
        {
            _formatter = formatter;
        }

        public IEnumerable<ComposedTopic> Compose(IEnumerable<TopicDescriptor> descriptors, IDirectory partialDirectory)
        {
            var topicItems = descriptors.ToList();
            var result = new List<ComposedTopic>();

            for (var placeLevel = 0; placeLevel <= topicItems.Max(t => t.Level); placeLevel++)
            {
                var level = placeLevel;
                var topicForLevel = topicItems.Where(t => t.Level == level).ToArray();
                foreach (var topicInfo in topicForLevel)
                {
                    IList<ComposedTopic> parent = result;
                    for (var i = 0; i < topicInfo.Path.Length; i++)
                    {
                        var placePart = topicInfo.Path[i];
                        var nextLevelParent = parent.FirstOrDefault(x => x.Key == placePart);
                        if (nextLevelParent == null)
                        {
                            var errorBuilder = new StringBuilder();
                            errorBuilder.AppendLine(string.Format("Topic definition for path [{0}] not found!", string.Join("/", topicInfo.Path.Take(i + 1))));
                            errorBuilder.AppendFormat("Docs content: {0}", topicInfo.ContentFile.AbsolutePath);

                            throw new Exception(errorBuilder.ToString());
                        }

                        parent = nextLevelParent.Children;
                    }

                    var item = new ComposedTopic
                    {
                        Descriptor = topicInfo
                    };

                    CreatePartialHtml(item, partialDirectory);

                    parent.Add(item);
                }
            }

            return result;
        }

        private void CreatePartialHtml(ComposedTopic item, IDirectory partialsDir)
        {
            var path = new List<string>(item.Descriptor.Path);
            path.Add(item.Descriptor.Key);

            var fileName = string.Join("__", path);
            fileName += ".html";
            var file = partialsDir.GetFile(fileName);
            var html = _formatter.Format(item.Descriptor.ContentFile.ReadAllText());

            var topicDocument = new HtmlDocument();
            topicDocument.LoadHtml(html);
            var headerNode = topicDocument.DocumentNode.SelectSingleNode("//h1");

            if (headerNode == null)
            {
                throw new Exception(
string.Format(@"Could not find H1 element in markup for topic.

Topic source file: {0}", item.Descriptor.ContentFile.AbsolutePath));
            }

            file.WriteStringToFile(topicDocument.DocumentNode.OuterHtml);

            item.Title = headerNode.InnerText;
            item.FormattedContentFile = file;
        }
    }
}