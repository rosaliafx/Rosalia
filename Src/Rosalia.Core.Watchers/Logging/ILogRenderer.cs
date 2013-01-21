namespace Rosalia.Core.Watchers.Logging
{
    using System;
    using Rosalia.Core.Logging;

    public interface ILogRenderer : IDisposable
    {
        void Init();

        void AppendMessage(int depth, string message, MessageLevel level);
    }
}