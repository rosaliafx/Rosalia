//namespace Rosalia.Core.Watchers.Visualization
//{
//    using System.Drawing;
//    using System.Drawing.Imaging;
//    using System.IO;
//    using System.Linq;
//    using Rosalia.Core.Watchers.Visualization.Layout;
//    using Rosalia.Core.Watching;
//
//    public class VisualizeWatcher : IWorkflowWatcher
//    {
//        public void DrawCenteredText(string text, Font font, Brush brush, Graphics graphics, RectangleF rectangle)
//        {
//            var measuredSize = graphics.MeasureString(text, font);
//
//            graphics.DrawString(
//                text,
//                font,
//                brush,
//                rectangle.Left + (rectangle.Width - measuredSize.Width) / 2,
//                rectangle.Top + (rectangle.Height - measuredSize.Height) / 2);
//                
//        }
//        public void Register(IWorkflowEventsAware eventsAware)
//        {
//            var taskIndex = 1;
//            var rootComposite = (Composite) null;
//            var currentComposite = (Composite) null;
//
//            eventsAware.TaskExecuting += (sender, args) =>
//            {
//                var composite = new Composite(args.CurrentTask, currentComposite);
//                composite.Order = taskIndex;
//                taskIndex++;
//
//                if (currentComposite != null)
//                {
//                    currentComposite.AddChild(composite);
//                }
//
//                currentComposite = composite;
//
//                if (rootComposite == null)
//                {
//                    rootComposite = currentComposite;
//                }
//            };
//
//            eventsAware.TaskExecuted += (sender, args) =>
//            {
//                currentComposite.Result = args.Result;
//                currentComposite = currentComposite.Parent;
//            };
//
//            eventsAware.WorkflowExecuted += (sender, args) =>
//            {
//                var font = new Font(FontFamily.GenericMonospace, 12);
//                var orderFont = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);
//
//                LayoutData layoutData = null;
//
//                using (var tempBitmap = new Bitmap(1, 1))
//                {
//                    using (var tempGraphics = Graphics.FromImage(tempBitmap))
//                    {
//                        var layoutManager = new LayoutManager(new GrapthRenderer(tempGraphics, font))
//                        {
//                            HorisontalMargin = 20,
//                            VerticalMargin = 30,
//                            LastLayerLeftMargin = 0
//                        };
//
//
//                        layoutData = layoutManager.CalcLayout(rootComposite);
//                    }
//                }
//
//                using (var writer = File.CreateText("test.svg"))
//                {
//                    writer.WriteLine("<svg xmlns='http://www.w3.org/2000/svg' version='1.1'>");
//                    foreach (var layoutItem in layoutData.AllItems)
//                    {
//                        var composite = (Composite) layoutItem.Data;
//                        writer.WriteLine(
//                            "<rect x='{0}' y ='{1}' width='{2}' height='{3}' style='fill:blue;stroke:pink;stroke-width:1;' />",
//                            (int) layoutItem.Position.X,
//                            (int) layoutItem.Position.Y,
//                            (int) layoutItem.Size.Width,
//                            (int) layoutItem.Size.Height);
//
//                        writer.WriteLine(
//                            "<text x='{0}' y ='{1}' text-anchor='middle' style='stroke:black;stroke-width:1;' font-family = 'monospace' >{2}</text>",
//                            (int) (layoutItem.Position.X + layoutItem.Size.Width / 2),
//                            (int) (layoutItem.Position.Y + layoutItem.Size.Height / 4),
//                            composite.Task.Name);
//
//                        writer.WriteLine(
//                            "<text x='{0}' y ='{1}' text-anchor='middle' style='stroke:black;stroke-width:1;' font-family = 'monospace' >{2}</text>",
//                            (int) (layoutItem.Position.X + layoutItem.Size.Width / 2),
//                            (int) (layoutItem.Position.Y + 3 * layoutItem.Size.Height / 4),
//                            composite.Order);
//                    }
//
//                    foreach (var link in layoutData.AllLinks)
//                    {
//                        writer.WriteLine("<polyline fill='none' stroke='black' stroke-width='1' points='");
//
//                        foreach (var point in link.Points)
//                        {
//                            writer.WriteLine("{0},{1}", (int)point.X, (int)point.Y);
//                        }
//
//                        writer.WriteLine("'/>");
//                    }
//
//                    writer.WriteLine("</svg>");
//                }
//
//                using (var bitmap = new Bitmap((int) layoutData.AllItems.Max(x => x.Position.X + x.Size.Width), (int) layoutData.AllItems.Max(x => x.Position.Y + x.Size.Width)))
//                {
//                    using (var g = Graphics.FromImage(bitmap))
//                    {
//                        foreach (var item in layoutData.AllItems)
//                        {
//                            var brush = ((Composite) item.Data).Result.ResultType == ResultType.Success
//                                ? Brushes.LightGreen : Brushes.IndianRed;
//
//                            var pen = ((Composite) item.Data).Result.ResultType == ResultType.Success
//                                ? Pens.Green : Pens.Brown;
//
//                            g.FillRectangle(brush, (int)item.Position.X, (int)item.Position.Y, (int)item.Size.Width, (int)item.Size.Height);
//                            g.DrawRectangle(pen, (int)item.Position.X, (int)item.Position.Y, (int)item.Size.Width, (int)item.Size.Height);
//
//                            DrawCenteredText(
//                                ((Composite)item.Data).Task.Name,
//                                font,
//                                Brushes.Black,
//                                g,
//                                new RectangleF(
//                                    (float)(item.Position.X),
//                                    (float)(item.Position.Y),
//                                    (float) item.Size.Width,
//                                    (float) item.Size.Height /2));
//
//                            DrawCenteredText(
//                                ((Composite)item.Data).Order.ToString(),
//                                orderFont,
//                                Brushes.Black,
//                                g,
//                                new RectangleF(
//                                    (float)(item.Position.X),
//                                    (float)(item.Position.Y + item.Size.Height / 2),
//                                    (float) item.Size.Width,
//                                    (float) item.Size.Height /2));
//                        }
//
//                        foreach (var link in layoutData.AllLinks)
//                        {
//                            g.DrawLines(
//                                Pens.SlateGray,
//                                link.Points.Select(l => new PointF((float)l.X, (float)l.Y)).ToArray());
//
//                            g.FillEllipse(
//                                Brushes.Gray,
//                                (float)(link.Points.First().X - 2),
//                                (float)(link.Points.First().Y - 2),
//                                4,
//                                4);
//                        }
//                    }
//
//                    using(var fileStream = File.Create("test.png"))
//                    {
//                        bitmap.Save(fileStream, ImageFormat.Png);
//                    }
//                }
//            };
//        }
//    }
//}