namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System;

    public class GridCoordTransformer : ICoordTransformer
    {
        private readonly Size _gridStepSize;

        public GridCoordTransformer(Size gridStepSize)
        {
            _gridStepSize = gridStepSize;
        }

        public void Init(LayoutData data)
        {
        }

        public void Transform(Point point)
        {
            point.X = GetGridCoord(point.X, _gridStepSize.Width);
            point.Y = GetGridCoord(point.Y, _gridStepSize.Height);
        }

        private static double GetGridCoord(double coord, double step)
        {
            var division = coord / step;
            var diff = division - ((int) division);
            if (Math.Abs(diff) > 0.00000001)
            {
                return step * Math.Ceiling(division);    
            }

            return coord;
        }
    }
}