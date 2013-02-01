namespace Rosalia.Build.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HtmlAgilityPack;
    using MarkdownSharp;
    using Rosalia.Build.DocSupport;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Tasks;

    public class BuildDocsTask : AbstractLeafTask<BuildRosaliaContext>
    {
        public BuildDocsTask()
        {
            Markdown = new Markdown();
            TopicRenderer = new DefaultTopicRenderer();
        }

        public Markdown Markdown { get; set; }

        public ITopicRenderer TopicRenderer { get; set; }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<BuildRosaliaContext> context)
        {
            var topicItems = GetTopicItems(context);

            var tableOfContents = new List<TableOfContentsItem>();
            for (var placeLevel = 0; placeLevel <= topicItems.Max(t => t.Level); placeLevel++)
            {
                var level = placeLevel;
                var topicForLevel = topicItems.Where(t => t.Level == level).ToArray();
                foreach (var topicInfo in topicForLevel)
                {
                    IList<TableOfContentsItem> parent = tableOfContents;
                    for (var i = 0; i < topicInfo.Path.Length; i++)
                    {
                        var placePart = topicInfo.Path[i];
                        var nextLevelParent = parent.FirstOrDefault(x => x.Key == placePart);
                        if (nextLevelParent == null)
                        {
                            var errorBuilder = new StringBuilder();
                            errorBuilder.AppendLine(string.Format("Topic definition for path [{0}] not found!", string.Join("/", topicInfo.Path.Take(i + 1))));
                            errorBuilder.AppendFormat("Docs root: {0}", topicInfo.Docs.AbsolutePath);
                            errorBuilder.AppendLine();
                            errorBuilder.AppendFormat("Docs content: {0}", topicInfo.ContentFile.AbsolutePath);

                            throw new Exception(errorBuilder.ToString());
                        }

                        parent = nextLevelParent.Children;
                    }

                    parent.Add(new TableOfContentsItem
                    {
                        Key = topicInfo.Key,
                        TopicItem = topicInfo
                    });
                }
            }

            var docHoshtProject = context.Data.Src.GetDirectory("Rosalia.Docs");
            var docTargetDir = docHoshtProject.GetDirectory("content");
            var partialsDir = docTargetDir.GetDirectory("partial");

            CreatePartialHtml(tableOfContents, partialsDir);
            CreateFullHtml(tableOfContents, tableOfContents, docTargetDir.GetDirectory("html"), docTargetDir);
        }

        private void CreateFullHtml(
            IList<TableOfContentsItem> allItems, 
            IEnumerable<TableOfContentsItem> items, 
            IDirectory targetHtmlDirectory, 
            IDirectory docTargetDir)
        {
            foreach (var item in items)
            {
                TopicRenderer.Render(item, allItems, targetHtmlDirectory.GetFile(item.ContentFile.Name), docTargetDir);
                if (item.Children.Count > 0)
                {
                    CreateFullHtml(allItems, item.Children, targetHtmlDirectory, docTargetDir);
                }
            }
        }

        private void CreatePartialHtml(IEnumerable<TableOfContentsItem> items, IDirectory partialsDir)
        {
            foreach (var item in items)
            {
                var path = new List<string>(item.TopicItem.Path);
                path.Add(item.TopicItem.Key);

                var fileName = string.Join("__", path);
                fileName += ".html";
                var file = partialsDir.GetFile(fileName);

                var html = Markdown.Transform(item.TopicItem.ContentFile.ReadAllText());
                file.WriteStringToFile(html);

                if (item.Children.Count > 0)
                {
                    CreatePartialHtml(item.Children, partialsDir);
                }

                var topicDocument = new HtmlDocument();
                topicDocument.LoadHtml(html);
                var headerNode = topicDocument.DocumentNode.SelectSingleNode("//h1");

                if (headerNode == null)
                {
                    throw new Exception(
string.Format(@"Could not find H1 element in markup for topic.

Topic source file: {0}", item.TopicItem.ContentFile.AbsolutePath));
                }

                item.Title = headerNode.InnerText;
                item.ContentFile = file;
            }
        }

        private static List<TopicItem> GetTopicItems(TaskContext<BuildRosaliaContext> context)
        {
            var topicItems = new List<TopicItem>();

            var directories = context.Data.Src.Directories.Where(d => d.Name.StartsWith("Rosalia."));
            foreach (var directory in directories)
            {
                var docsDirectory = directory.GetDirectory("Docs");
                if (docsDirectory.Exists)
                {
                    var topicFiles = context.FileSystem.SearchFilesIn(docsDirectory).Include(file => file.Name.EndsWith(".md"));
                    foreach (var file in topicFiles)
                    {
                        topicItems.Add(new TopicItem(docsDirectory, file));
                    }
                }
            }

            return topicItems;
        }
    }
}