namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LayoutManager
    {
        private readonly IRenderer _renderer;

        public double HorisontalMargin { get; set; }

        public double VerticalMargin { get; set; }

        public double LastLayerLeftMargin { get; set; }

        public bool ReorderLastLayer { get; set; }

        public IList<ICoordTransformer> CoordTransformers { get; set; }

        public LayoutManager(IRenderer renderer)
        {
            _renderer = renderer;
            CoordTransformers = new List<ICoordTransformer>();
        }

        public LayoutData CalcLayout(IComposite composite)
        {
            var result = new LayoutData();
            FillLayers(result, composite);
            CalcInitialItemsSize(result);
            CalcBound(result.AllItems.First());
            CalcPositionByBounds(0, 0, result.AllItems.First());
            FillLinks(result);
            TransformCoords(result);

            return result;
        }

        private void TransformCoords(LayoutData result)
        {
            foreach (var coordTransformer in CoordTransformers)
            {
                coordTransformer.Init(result);
            }

            foreach (var layoutItem in result.AllItems)
            {
                foreach (var coordTransformer in CoordTransformers)
                {
                    coordTransformer.Transform(layoutItem.Position);
                }
            }

            foreach (var link in result.AllLinks)
            {
                foreach (var point in link.Points)
                {
                    foreach (var coordTransformer in CoordTransformers)
                    {
                        coordTransformer.Transform(point);
                    }
                }
            }
        }

        private void FillLinks(LayoutData result)
        {
            FillLinks(result, result.AllItems.First());
        }

        private void FillLinks(LayoutData result, LayoutItem item)
        {
            foreach (var child in item.Children)
            {
                var link = new Link();
                item.OutgoingLinks.Add(link);
                child.IncomingLinks.Add(link);
                result.AllLinks.Add(link);
                CalcLinkPosition(item, child, link, result);
                FillLinks(result, child);
            }
        }

        private void CalcLinkPosition(LayoutItem from, LayoutItem to, Link link, LayoutData layoutData)
        {
            if (_renderer.GetChildrenLayoutType(to.Parent) == LayoutType.Vertical)
            {
                var fromPoint = link.CreatePoint();
                //fromPoint.X = from.Position.X + LastLayerLeftMargin / 2;
                fromPoint.X = from.Position.X + (to.Position.X - from.Position.X) / 2;
                fromPoint.Y = from.Position.Y + from.Size.Height;

                var middlePoint = link.CreatePoint();

                var toPoint = link.CreatePoint();
                toPoint.X = to.Position.X;
                toPoint.Y = to.Position.Y + to.Size.Height / 2;

                middlePoint.X = fromPoint.X;
                middlePoint.Y = toPoint.Y;
            }
            else
            {
                var fromPoint = link.CreatePoint();
                fromPoint.X = from.Position.X + from.Size.Width / 2;
                fromPoint.Y = from.Position.Y + from.Size.Height;

                var middlePoint1 = link.CreatePoint();
                var middlePoint2 = link.CreatePoint();

                var toPoint = link.CreatePoint();
                toPoint.X = to.Position.X + to.Size.Width / 2;
                toPoint.Y = to.Position.Y;

                middlePoint1.X = fromPoint.X;
                middlePoint1.Y = fromPoint.Y + (toPoint.Y - fromPoint.Y) / 2;

                middlePoint2.X = toPoint.X;
                middlePoint2.Y = fromPoint.Y + (toPoint.Y - fromPoint.Y) / 2;
            }
        }

        public void CalcBound(LayoutItem item)
        {
            var itemWidthHalf = item.Size.Width / 2d;
            var itemHeightHalf = item.Size.Height / 2d;

            if (item.Children.Count == 0)
            {
                item.Bound.Right = itemWidthHalf + HorisontalMargin;
                item.Bound.Left = GetCurrentLeftMargin(item);
                item.Bound.Top = itemHeightHalf;
                item.Bound.Bottom = itemHeightHalf + VerticalMargin / 2;

                return;
            }

            foreach (var child in item.Children)
            {
                CalcBound(child);
            }

            var childrenLayoutType = _renderer.GetChildrenLayoutType(item);
            if (childrenLayoutType == LayoutType.Vertical)
            {
                var a = itemWidthHalf - LastLayerLeftMargin - itemWidthHalf;
                item.Bound.Right = Math.Max(
                    itemWidthHalf + HorisontalMargin,
                    item.Children.Max(c => c.Bound.Width - a));
                item.Bound.Left = GetCurrentLeftMargin(item);
                item.Bound.Top = itemHeightHalf;
                item.Bound.Bottom = itemHeightHalf + VerticalMargin / 2 + item.Children.Sum(c => c.Size.Height / 2 + c.Bound.Bottom);
            }
            else
            {
                var childrenFullWidth = item.Children.Sum(c => c.Bound.Width);
                var clientFullWidth = childrenFullWidth - item.Children.First().Bound.Left - item.Children.Last().Bound.Right;
                item.Bound.Right = Math.Max(
                    itemWidthHalf + HorisontalMargin,
                    clientFullWidth / 2 + item.Children.Last().Bound.Right);
                item.Bound.Left = Math.Max(
                    GetCurrentLeftMargin(item),
                    clientFullWidth / 2 + item.Children.First().Bound.Left);
                item.Bound.Top = itemHeightHalf;
                item.Bound.Bottom = itemHeightHalf + VerticalMargin + item.Children.Max(c => c.Bound.Height);
            }
        }

        private void CalcPositionByBounds(double left, double top, LayoutItem item)
        {
            item.Position.X = left + item.Bound.Left - item.Size.Width / 2d;
            item.Position.Y = top;

            if (item.Children.Count == 0)
            {
                return;
            }

            var isVertical = _renderer.GetChildrenLayoutType(item) == LayoutType.Vertical;
            var paddingTopVertical = top + item.Size.Height + VerticalMargin / 2;
            var paddingLeftHorizontal = left;

            var horisontalDiffRight = 0d;
            if (isVertical)
            {
                // Старый метод расчета.
                // horisontalDiffRight = (item.Size.Width - item.Size.Width / 2 - LastLayerLeftMargin - item.Children.Max(c => c.Bound.Width)) / 2;
            }
            else
            {
                horisontalDiffRight = (item.Size.Width - item.Children.Sum(c => c.Bound.Width)) /
                                      (item.Children.Count + 1);
            }
            if (horisontalDiffRight < 0)
            {
                horisontalDiffRight = 0;
            }

            foreach (var child in item.Children)
            {
                if (isVertical)
                {
                    CalcPositionByBounds(item.Position.X + LastLayerLeftMargin + horisontalDiffRight + item.Size.Width / 2, paddingTopVertical, child);
                    paddingTopVertical += child.Bound.Height;
                }
                else
                {
                    CalcPositionByBounds(paddingLeftHorizontal + horisontalDiffRight, top + item.Size.Height + VerticalMargin, child);
                    paddingLeftHorizontal += child.Bound.Width;
                }
            }
        }

        private double GetCurrentLeftMargin(LayoutItem item)
        {
            if (item.Parent == null)
            {
                // Тут может использоваться другой метод расчета.
                return item.Size.Width / 2d;
            }

            return item.Size.Width / 2d;
        }

        private void CalcInitialItemsSize(LayoutData result)
        {
            foreach (var item in result.AllItems)
            {
                item.Size = _renderer.GetItemSize(item);
            }
        }

        public static void FillLayers(LayoutData data, IComposite composite)
        {
            var firstLayer = data.CreateNewLayer();
            FillLayers(data, firstLayer, null, composite);
        }

        public static void FillLayers(LayoutData data, Layer currentLayer, LayoutItem parent, IComposite composite)
        {
            var current = new LayoutItem(composite, parent);
            currentLayer.AddItem(current);
            if (composite.ChildrenItems.Count() == 0)
            {
                return;
            }

            var nextLayer = data.GetNextLayer(currentLayer);
            foreach (var child in composite.ChildrenItems)
            {
                FillLayers(data, nextLayer, current, child);
            }
        }
    }
}