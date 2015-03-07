namespace Rosalia.TestingSupport
{
    using System;
    using Rosalia.Core;
    using Rosalia.Core.Logging;

    public class SimpleLogRenderer : ILogRenderer
    {
        public void Init()
        {
        }

        public void Render(Message message, Identity source)
        {
            Console.WriteLine("[{0}] - [{1}] - {2}", source.Value, message.Level, message.Text);
        }

        public void Dispose()
        {
        }
    }
}