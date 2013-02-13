namespace Rosalia.Build.DocSupport
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Rosalia.Core.FileSystem;

    public class DefaultTopicRenderer : ITopicRenderer
    {
        private string Template =
@"<!DOCTYPE html><html>
    <head>
        <title>Rosalia Docs - {{Title}}</title>
        {{#Styles}}
            <link href='{{Source}}' rel='stylesheet' />
        {{/Styles}}
    </head>
    <body>
        <div id='page'>
            <header class='navbar navbar-inverse navbar-fixed-top'>
                <div class='container'>
                    <a id='mainLogo' href='{{PathToRoot}}index.html'>
                        <span>Rosalia</span><span class='docs'>Docs</span>
                    </a>
                </div>
            </header>

            <div class='container'>
                <section>
                    <nav id='tableOfContents' class='affix'>
                        {{{TableOfContents}}}
                    </nav>
                </section>
                <section id='mainContent'>
                    {{{MainContent}}}
                </section>
            </div>
        </div>
        <footer></footer>
    </body>

    {{#Scripts}}
        <script type='text/javascript' src='{{Source}}'></script>
    {{/Scripts}}
    <script>hljs.initHighlightingOnLoad();</script>
</html>";

        public void Render(
            ComposedTopic topic, 
            IEnumerable<ComposedTopic> allTopics, 
            IFile output,
            IDirectory hostDirectory,
            IDirectory stylesDirectory,
            IDirectory scriptsDirectory)
        {
            var topicDirectory = hostDirectory.GetFile(topic.RelativeResultFilePath).Directory;

            var pathToRoot = string.Empty;
            for (var i = 0; i < topic.Descriptor.Level + 1; i++)
            {
                pathToRoot += "../";
            }

            var model = new Model
            {
                PathToRoot = pathToRoot,
                TableOfContents = GetTableOfContentsMarkup(allTopics, topic, topicDirectory),
                Title = topic.Title,
                MainContent = topic.FormattedContentFile.ReadAllText(),
                Scripts = scriptsDirectory
                    .Files
                    .Select(file => new Resource(file.GetRelativePath(topicDirectory).Replace("\\", "/")))
                    .ToList(),
                Styles = stylesDirectory
                    .Files
                    .Select(file => new Resource(file.GetRelativePath(topicDirectory).Replace("\\", "/")))
                    .ToList()
            };

            var result = Nustache.Core.Render.StringToString(Template, model);

            output.WriteStringToFile(result);
        }

        private string GetTableOfContentsMarkup(IEnumerable<ComposedTopic> allTopics, ComposedTopic topic, IDirectory topicDirectory)
        {
            var builder = new StringBuilder();
            RenderTableOfContentsLevel(builder, allTopics, topic, topicDirectory);
            return builder.ToString();
        }

        private void RenderTableOfContentsLevel(StringBuilder builder, IEnumerable<ComposedTopic> allTopics, ComposedTopic current, IDirectory pathToRoot)
        {
            builder.AppendLine("<ul>");
            var currentTopicDirectory = pathToRoot.GetFile(current.RelativeResultFilePath).Directory;

            foreach (var topic in allTopics)
            {
                var topicFile = pathToRoot.GetFile(topic.RelativeResultFilePath).GetRelativePath(currentTopicDirectory);

                builder.AppendLine("<li>");
                if (topic == current)
                {
                    builder.AppendFormat("<span>{0}</span>", topic.Title);
                } else
                {
                    builder.AppendFormat("<a href='{0}'>{1}</a>", topicFile.Replace("\\", "/"), topic.Title);
                }

                builder.AppendLine();

                if (topic.Children.Count > 0)
                {
                    RenderTableOfContentsLevel(builder, topic.Children, current, pathToRoot);
                }

                builder.AppendLine("</li>");
            }

            builder.AppendLine("</ul>");
        }

        internal class Model
        {
            public string PathToRoot { get; set; }

            public string Title { get; set; }

            public string TableOfContents { get; set; }

            public string MainContent { get; set; }

            public IList<Resource> Styles { get; set; }

            public IList<Resource> Scripts { get; set; }
        }

        internal class Resource
        {
            public Resource(string source)
            {
                Source = source;
            }

            public string Source { get; private set; }
        }
    }
}