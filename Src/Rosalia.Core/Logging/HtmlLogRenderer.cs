namespace Rosalia.Core.Logging
{
    using System;
    using System.IO;

    public class HtmlLogRenderer : ILogRenderer
    {
        private readonly Lazy<TextWriter> _writerProvider;

        public HtmlLogRenderer(Lazy<TextWriter> writerProvider)
        {
            _writerProvider = writerProvider;
        }

        public void Init()
        {
            _writerProvider.Value.WriteLine(
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
<body><table id='log'>
<thead>
    <tr>
        <th>Level</th>
        <th>Task</th>
        <th>Message</th>
    </tr>
</thead>
<tbody>");
        }

        public void Render(Message message, Identity source)
        {
            _writerProvider.Value.WriteLine("<tr class='item {0}'>", message.Level);
            _writerProvider.Value.WriteLine("<td>{0}</td>", message.Level);
            _writerProvider.Value.WriteLine("<td>{0}</td>", source.Value);
            _writerProvider.Value.WriteLine("<td>{0}</td>", message.Text);
//            _writerProvider.Value.WriteLine("<span class='message'><pre>{0}</pre></span>", 
//                //HttpUtility.HtmlEncode(message)
//                message.Text); //todo
            _writerProvider.Value.WriteLine("</tr>");
        }

        public void Dispose()
        {
            _writerProvider.Value.WriteLine(
@"</tbody></table></body></html>");
            _writerProvider.Value.Flush();
            _writerProvider.Value.Close();
        }
    }
}