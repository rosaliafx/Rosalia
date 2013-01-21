namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System.Collections.Generic;
    using System.Linq;

    public class Layer
    {
        private readonly IList<LayoutItem> _items;

        public IEnumerable<LayoutItem> Items
        {
            get
            {
                return _items;
            }
        }

        public Layer()
        {
            _items = new List<LayoutItem>();
        }

        public void AddItem(LayoutItem item)
        {
            _items.Add(item);
        }

        public double CalcHeight()
        {
            return _items.Max(i => i.Size.Height);
        }
    }
}