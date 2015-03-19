namespace Rosalia.Core.Logging
{
    using System;

    public interface ILogRenderer : IDisposable
    {
        void Init();

        void Render(Message message, Identity source);
    }
}