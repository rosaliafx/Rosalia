namespace Rosalia.Core.Watchers.Visualization
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Rosalia.Core.Watchers.Visualization.Layout;

    public class GraphicsVisualizeWatcher : AbstractVisualizeWatcher
    {
        private readonly string _outputPath;
        private Graphics _graphics;
        private Bitmap _bitmap;
        private Font _titleFont;
        private Font _orderFont;

        public GraphicsVisualizeWatcher(string outputPath)
        {
            _outputPath = outputPath;
        }
        
        protected override void RenderEnd()
        {
            using (var fileStream = File.Create(_outputPath))
            {
                _bitmap.Save(fileStream, ImageFormat.Png);
            }
        }

        protected override void RenderStart(LayoutData layoutData)
        {
        }

        protected override void RenderLink(Link link)
        {
            var points = link.Points.Select(l => new PointF((float) l.X, (float) l.Y)).ToArray();

            _graphics.DrawLines(
                Pens.SlateGray,
                points);
            
            _graphics.FillEllipse(
                Brushes.Gray,
                (float)(link.Points.First().X - 2),
                (float)(link.Points.First().Y - 2),
                4,
                4);
        }

        protected override void RenderItem(Composite data, LayoutItem item)
        {
            var brush = GetTaskBrush(data);
            var pen = GetTaskPen(data);

            _graphics.FillRectangle(brush, (int)item.Position.X, (int)item.Position.Y, (int)item.Size.Width, (int)item.Size.Height);
            _graphics.DrawRectangle(pen, (int)item.Position.X, (int)item.Position.Y, (int)item.Size.Width, (int)item.Size.Height);

            DrawCenteredText(
                data.Task.Name,
                _titleFont,
                Brushes.Black,
                _graphics,
                new RectangleF(
                    (float)(item.Position.X),
                    (float)(item.Position.Y),
                    (float)item.Size.Width,
                    (float)item.Size.Height / 2));

            DrawCenteredText(
                data.Result == null ? "skipped" : data.Order.ToString(),
                _orderFont,
                Brushes.Black,
                _graphics,
                new RectangleF(
                    (float)(item.Position.X),
                    (float)(item.Position.Y + item.Size.Height / 2),
                    (float)item.Size.Width,
                    (float)item.Size.Height / 2));
        }

        protected override IDisposable[] AllocateResources(LayoutData layoutData)
        {
            var width = (int)layoutData.AllItems.Max(x => x.Position.X + x.Size.Width);
            var height = (int)layoutData.AllItems.Max(x => x.Position.Y + x.Size.Height);

            _bitmap = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            _titleFont = new Font(FontFamily.GenericMonospace, 12);
            _orderFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);

            return new IDisposable[]
            {
                _titleFont,
                _orderFont,
                _graphics,
                _bitmap
            };
        }

        private static Pen GetTaskPen(Composite data)
        {
            if (data.Result == null)
            {
                return Pens.Gray;
            }

            if (data.Result.ResultType == ResultType.Success)
            {
                return Pens.Green;
            }

            return Pens.Brown;
        }

        private static Brush GetTaskBrush(Composite data)
        {
            if (data.Result == null)
            {
                return Brushes.LightGray;
            }

            if (data.Result.ResultType == ResultType.Success)
            {
                return Brushes.LightGreen;
            }

            return Brushes.IndianRed;
        }

        private void DrawCenteredText(string text, Font font, Brush brush, Graphics graphics, RectangleF rectangle)
        {
            var measuredSize = graphics.MeasureString(text, font);

            graphics.DrawString(
                text,
                font,
                brush,
                rectangle.Left + (rectangle.Width - measuredSize.Width) / 2,
                rectangle.Top + (rectangle.Height - measuredSize.Height) / 2);
        }
    }
}