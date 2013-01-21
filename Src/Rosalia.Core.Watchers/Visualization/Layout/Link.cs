namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System.Collections.Generic;
    using System.Linq;

    public class Link
    {
        private readonly IList<Point> _points;

        public Link()
        {
            _points = new List<Point>();
        }

        public IEnumerable<Point> Points
        {
            get
            {
                return _points;
            }
        }

        public Point CreatePoint()
        {
            var point = new Point();
            _points.Add(point);
            return point;
        }

        public double Width
        {
            get
            {
                return Points.Last().X - Points.First().X;
            }
        }

        public double Height
        {
            get
            {
                return Points.Last().Y - Points.First().Y;
            }
        }
    }
}