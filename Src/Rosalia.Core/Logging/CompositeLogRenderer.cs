namespace Rosalia.Core.Logging
{
    public class CompositeLogRenderer : ILogRenderer
    {
        private readonly ILogRenderer[] _nestedRenderers;

        public CompositeLogRenderer(params ILogRenderer[] nestedRenderers)
        {
            _nestedRenderers = nestedRenderers;
        }

        public void Render(Message message, Identity source)
        {
            foreach (var nestedRenderer in _nestedRenderers)
            {
                nestedRenderer.Render(message, source);
            }
        }
    }
}