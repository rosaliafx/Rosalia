namespace Rosalia.Build.Tasks
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Build.DocSupport;
    using Rosalia.Build.DocSupport.Discovering;
    using Rosalia.Build.DocSupport.Formatting;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Tasks;

    public class BuildDocsTask : AbstractLeafTask
    {
        public BuildDocsTask()
        {
            TopicRenderer = new DefaultTopicRenderer();
            TopicSearcher = new SimpleTopicSearcher();
            TopicComposer = new DefaultTopicComposer(new MarkdownFormatter());
        }

        public ITopicRenderer TopicRenderer { get; set; }

        public ITopicSearcher TopicSearcher { get; set; }

        public ITopicComposer TopicComposer { get; set; }

        public IDirectory SourceDirectory { get; set; }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
        {
            var docHoshtProject = SourceDirectory.GetDirectory("Rosalia.Docs");
            var docTargetDir = docHoshtProject.GetDirectory("content");
            var partialsDir = docTargetDir.GetDirectory("partial");

            var directories = SourceDirectory.Directories.Where(d => d.Name.StartsWith("Rosalia."));
            var descriptors = TopicSearcher.Find(directories);
            var topics = TopicComposer.Compose(descriptors, partialsDir).ToList();

            CreateFullHtml(topics, topics, docHoshtProject);
        }

        private void CreateFullHtml(
            IList<ComposedTopic> allItems, 
            IEnumerable<ComposedTopic> items, 
            IDirectory docTargetDir)
        {
            foreach (var item in items)
            {
                var fullHtmlFile = docTargetDir.GetDirectory("topics").GetFile(item.RelativeResultFilePath);
                fullHtmlFile.EnsureExists();

                TopicRenderer.Render(
                    item, 
                    allItems, 
                    fullHtmlFile, 
                    docTargetDir.GetDirectory("topics"),
                    docTargetDir.GetDirectory("content/styles"),
                    docTargetDir.GetDirectory("content/scripts"));

                if (item.Children.Count > 0)
                {
                    CreateFullHtml(allItems, item.Children, docTargetDir);
                }
            }
        }
    }
}