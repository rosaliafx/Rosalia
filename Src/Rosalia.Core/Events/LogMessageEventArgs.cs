namespace Rosalia.Core.Events
{
    using System;
    using Rosalia.Core.Logging;

    public class LogMessageEventArgs : EventArgs
    {
        public string Template { get; set; }

        public object[] Args { get; set; }

        public MessageLevel Level { get; set; }
    }
}