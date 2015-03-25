namespace Rosalia.Core.Logging
{
    public class FilterLogRenderer : ILogRenderer
    {
        private readonly ILogRenderer _nestedRenderer;
        private readonly MessageLevel _level;

        public FilterLogRenderer(ILogRenderer nestedRenderer, MessageLevel level)
        {
            _nestedRenderer = nestedRenderer;
            _level = level;
        }

        public void Dispose()
        {
            _nestedRenderer.Dispose();
        }

        public void Init()
        {
            _nestedRenderer.Init();
        }

        public void Render(Message message, Identities source)
        {
            if (message.Level >= _level)
            {
                _nestedRenderer.Render(message, source);
            }
        }
    }
}