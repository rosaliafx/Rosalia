namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System.Linq;
    using System.Collections.Generic;

    public class LayoutItem
    {
        private readonly IList<LayoutItem> _children;

        private LayoutItem _parent;

        public LayoutItem(IComposite data, LayoutItem parent)
        {
            Data = data;

            Position = new Point();
            IncomingLinks = new List<Link>();
            OutgoingLinks = new List<Link>();
            Bound = new Bound();

            _children = new List<LayoutItem>();

            Parent = parent;
            if (parent != null)
            {
                parent.Children.Add(this);
            }
        }

        public IComposite Data { get; private set; }

        public Point Position { get; private set; }

        public Bound Bound { get; private set; }

        public IList<Link> IncomingLinks { get; private set; }

        public IList<Link> OutgoingLinks { get; private set; }

        public Size Size { get; set; }

        public double FullWidth { get; set; }

        public double ChildrenWidth { get; set; }

        public double FullHeight { get; set; }

        public bool IsPreLastNode
        {
            get
            {
                if (Children.Count == 0)
                {
                    return false;
                }

                return Children.All(child => child.Data.IsEmpty());
            }
        }

        public IList<LayoutItem> Children
        {
            get { return _children; }
        }

        public LayoutItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }
}