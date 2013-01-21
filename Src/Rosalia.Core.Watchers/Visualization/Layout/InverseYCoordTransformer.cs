namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System.Linq;

    public class InverseYCoordTransformer : ICoordTransformer
    {
        private double _fullHeight;

        public void Init(LayoutData data)
        {
            _fullHeight = data.AllItems.First().Bound.Height - 0.5;
        }

        public void Transform(Point point)
        {
            point.Y = _fullHeight - point.Y;
        }
    }
}