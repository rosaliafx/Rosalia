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
                    <a id='mainLogo' href='/index.html'>
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
    </body>

    {{#Scripts}}
        <script type='text/javascript' src='{{Source}}'></script>
    {{/Scripts}}
    <script>hljs.initHighlightingOnLoad();</script>
</html>";

        public void Render(
            TableOfContentsItem topic, 
            IEnumerable<TableOfContentsItem> allTopics, 
            IFile output,
            IDirectory hostDirectory)
        {
            var model = new Model
            {
                TableOfContents = GetTableOfContentsMarkup(allTopics, topic),
                Title = topic.Title,
                MainContent = topic.ContentFile.ReadAllText(),
                Scripts = hostDirectory
                    .GetDirectory("scripts")
                    .Files
                    .Select(file => new Resource("/content/scripts/" + file.Name))
                    .ToList(),
                Styles = hostDirectory
                    .GetDirectory("styles")
                    .Files
                    .Select(file => new Resource("/content/styles/" + file.Name))
                    .ToList()
            };

            var result = Nustache.Core.Render.StringToString(Template, model);

            output.WriteStringToFile(result);
        }

        private string GetTableOfContentsMarkup(IEnumerable<TableOfContentsItem> allTopics, TableOfContentsItem topic)
        {
            var builder = new StringBuilder();
            RenderTableOfContentsLevel(builder, allTopics, topic);
            return builder.ToString();
        }

        private void RenderTableOfContentsLevel(StringBuilder builder, IEnumerable<TableOfContentsItem> allTopics, TableOfContentsItem current)
        {
            builder.AppendLine("<ul>");
            foreach (var topic in allTopics)
            {
                builder.AppendLine("<li>");
                if (topic == current)
                {
                    builder.AppendFormat("<span>{0}</span>", topic.Title);
                } else
                {
                    builder.AppendFormat("<a href='/content/html/{0}'>{1}</a>", topic.ContentFile.Name, topic.Title);
                }

                builder.AppendLine();

                if (topic.Children.Count > 0)
                {
                    RenderTableOfContentsLevel(builder, topic.Children, current);
                }

                builder.AppendLine("</li>");
            }

            builder.AppendLine("</ul>");
        }

        internal class Model
        {
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