namespace Rosalia.TestingSupport
{
    using System;
    using System.Linq;
    using Rosalia.Core;
    using Rosalia.Core.Logging;

    public class SimpleLogRenderer : ILogRenderer
    {
        public void Init()
        {
        }

        public void Render(Message message, Identities source)
        {
            Console.WriteLine("[{0}] - [{1}] - {2}", string.Join(" -> ", source.Items.Select(id => id.Value).ToArray()), message.Level, message.Text);
        }

        public void Dispose()
        {
        }
    }
}