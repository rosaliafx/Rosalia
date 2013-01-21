namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LayoutData
    {
        private readonly IList<Layer> _layers;

        public IList<Link> AllLinks { get; private set; }

        public IEnumerable<Layer> Layers
        {
            get
            {
                return _layers;
            }
        }

        public IEnumerable<LayoutItem> AllItems
        {
            get
            {
                return Layers.SelectMany(layer => layer.Items);
            }
        }

        public LayoutData()
        {
            _layers = new List<Layer>();
            AllLinks = new List<Link>();
        }

        public Layer CreateNewLayer()
        {
            var newLayer = new Layer();
            _layers.Add(newLayer);
            return newLayer;
        }

        public Layer GetNextLayer(Layer layer)
        {
            var currentLayerIndex = _layers.IndexOf(layer);

            var layerNotFound = currentLayerIndex < 0;
            if (layerNotFound)
            {
                throw new Exception("Layer not found");
            }

            var isLastLayer = currentLayerIndex == _layers.Count - 1;
            if (isLastLayer)
            {
                return CreateNewLayer();
            }

            return _layers[currentLayerIndex + 1];
        }

        public LayoutItem GetParentItem(LayoutItem item)
        {
            return AllItems.FirstOrDefault(i => i.Data.ChildrenItems.Contains(item.Data));
        }
    }
}