namespace Rosalia.Core.Watchers.Logging
{
    using System.Collections.Generic;
    using Rosalia.Core.Logging;

    public class CompositeLogRenderer : ILogRenderer
    {
        private readonly IList<ILogRenderer> _nestedRenderers;

        public CompositeLogRenderer() : this(new List<ILogRenderer>())
        {
        }

        public CompositeLogRenderer(IList<ILogRenderer> nestedRenderers)
        {
            _nestedRenderers = nestedRenderers;
        }

        public void AddRenderer(ILogRenderer renderer)
        {
            _nestedRenderers.Add(renderer);
        }

        public void Init()
        {
            foreach (var renderer in _nestedRenderers)
            {
                renderer.Init();
            }
        }

        public void AppendMessage(int depth, string message, MessageLevel level)
        {
            foreach (var renderer in _nestedRenderers)
            {
                renderer.AppendMessage(depth, message, level);
            }
        }

        public void Dispose()
        {
            foreach (var renderer in _nestedRenderers)
            {
                renderer.Dispose();
            }
        }
    }
}