namespace Rosalia.Core.Logging
{
    public class CompositeLogRenderer : ILogRenderer
    {
        private readonly ILogRenderer[] _nestedRenderers;

        public CompositeLogRenderer(params ILogRenderer[] nestedRenderers)
        {
            _nestedRenderers = nestedRenderers;
        }

        public void Init()
        {
            foreach (var renderer in _nestedRenderers)
            {
                renderer.Init();
            }
        }

        public void Render(Message message, Identities source)
        {
            foreach (var nestedRenderer in _nestedRenderers)
            {
                nestedRenderer.Render(message, source);
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