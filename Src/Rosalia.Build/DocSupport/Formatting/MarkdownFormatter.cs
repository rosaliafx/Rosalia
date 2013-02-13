namespace Rosalia.Build.DocSupport.Formatting
{
    using MarkdownSharp;

    public class MarkdownFormatter : IDocFormatter
    {
        private readonly Markdown _markdown = new Markdown();

        public string Format(string source)
        {
            return _markdown.Transform(source);
        }
    }
}