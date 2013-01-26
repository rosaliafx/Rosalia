namespace Rosalia.Core.Events
{
    using System;
    using Rosalia.Core.Logging;

    public class TaskMessageEventArgs : EventArgs
    {
        public TaskMessageEventArgs(Message message, IIdentifiable source)
        {
            Message = message;
            Source = source;
        }

        public Message Message { get; private set; }

        public IIdentifiable Source { get; private set; }
    }
}