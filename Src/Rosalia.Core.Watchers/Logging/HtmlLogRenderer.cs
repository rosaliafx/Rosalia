namespace Rosalia.Core.Watchers.Logging
{
    using System.IO;
    using System.Web;
    using Rosalia.Core.Logging;

    public class HtmlLogRenderer : ILogRenderer
    {
        private readonly TextWriter _writer;

        public HtmlLogRenderer(TextWriter writer)
        {
            _writer = writer;
        }

        public void Init()
        {
            _writer.WriteLine(
@"<!doctype html>
<html>
<head>
    <style type='text/css'>
        #log .item
        {
            overflow: hidden;
        }

        #log .item SPAN
        {
            float: left;
        }

        #log .separator
        {
            width: 20px;
        }

        #log .item PRE
        {
            padding: 0;
            margin: 0;
        }
    </style>
</head>
<body><div id='log'>");
        }

        public void AppendMessage(int depth, string message, MessageLevel level)
        {
            _writer.WriteLine("<div class='item {0}'>", level);
            for (var i = 0; i < depth; i++)
            {
                _writer.WriteLine("<span class='separator'>&nbsp;</span>");
            }

            _writer.WriteLine("<span class='message'><pre>{0}</pre></span>", HttpUtility.HtmlEncode(message));
            _writer.WriteLine("</div>");
        }

        public void Dispose()
        {
            _writer.WriteLine(
@"</div></body></html>");
            _writer.Flush();
            _writer.Close();
        }
    }
}