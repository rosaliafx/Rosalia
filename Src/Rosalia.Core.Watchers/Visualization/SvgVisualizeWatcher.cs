namespace Rosalia.Core.Watchers.Visualization
{
    using System;
    using System.IO;
    using Rosalia.Core.Watchers.Visualization.Layout;

    public class SvgVisualizeWatcher : AbstractVisualizeWatcher
    {
        private readonly string _outputPath;

        private TextWriter _outputWriter;

        public SvgVisualizeWatcher(string outputPath)
        {
            _outputPath = outputPath;
        }

        protected override void RenderEnd()
        {
            _outputWriter.WriteLine("</svg>");
        }

        protected override void RenderStart(LayoutData layoutData)
        {
            _outputWriter.WriteLine("<svg xmlns='http://www.w3.org/2000/svg' version='1.1'>");
        }

        protected override void RenderLink(Link link)
        {
            _outputWriter.WriteLine("<polyline fill='none' stroke='black' stroke-width='0.5' points='");

            foreach (var point in link.Points)
            {
                _outputWriter.WriteLine("{0},{1}", (int)point.X, (int)point.Y);
            }

            _outputWriter.WriteLine("'/>");
        }

        protected override void RenderItem(Composite data, LayoutItem layoutItem)
        {
            _outputWriter.WriteLine(
                "<rect x='{0}' y ='{1}' width='{2}' height='{3}' style='fill:#{4};stroke:#{5};stroke-width:1;' />",
                (int)layoutItem.Position.X,
                (int)layoutItem.Position.Y,
                (int)layoutItem.Size.Width,
                (int)layoutItem.Size.Height,
                GetTaskFillColor(data),
                GetTaskStrokeColor(data));

            _outputWriter.WriteLine(
                "<text x='{0}' y ='{1}' text-anchor='middle' style='stroke:black;stroke-width:1;' font-family='monospace' >{2}</text>",
                (int)(layoutItem.Position.X + layoutItem.Size.Width / 2),
                (int)(layoutItem.Position.Y + layoutItem.Size.Height / 4),
                data.Task.Name);

            _outputWriter.WriteLine(
                "<text x='{0}' y ='{1}' text-anchor='middle' style='stroke:black;stroke-width:1;' font-family='monospace' >{2}</text>",
                (int)(layoutItem.Position.X + layoutItem.Size.Width / 2),
                (int)(layoutItem.Position.Y + 3 * layoutItem.Size.Height / 4),
                data.Order);
        }

        private string GetTaskStrokeColor(Composite data)
        {
            if (data.Result == null)
            {
                return "808080";
            }

            if (data.Result.ResultType == ResultType.Success)
            {
                return "1b6d1b";
            }

            return "9f2c2f";
        }

        private string GetTaskFillColor(Composite data)
        {
            if (data.Result == null)
            {
                return "d3d3d3";
            }

            if (data.Result.ResultType == ResultType.Success)
            {
                return "90ee90";
            }

            return "cd5d5c";
        }

        protected override IDisposable[] AllocateResources(LayoutData layoutData)
        {
            _outputWriter = File.CreateText(_outputPath);

            return new IDisposable[] { _outputWriter };
        }
    }
}