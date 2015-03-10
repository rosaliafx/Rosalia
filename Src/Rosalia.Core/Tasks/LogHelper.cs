namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Logging;

    public class LogHelper
    {
        private readonly Action<Message> _callback;

        public LogHelper(Action<Message> callback)
        {
            _callback = callback;
        }

        public void Info(string mesage, params object[] args)
        {
            AddMessage(MessageLevel.Info, mesage, args);
        }

        public void Warning(string text, params object[] args)
        {
            AddMessage(MessageLevel.Warn, text, args);
        }

        public void Error(string text, params object[] args)
        {
            AddMessage(MessageLevel.Error, text, args);
        }

        public void AddMessage(MessageLevel level, string text, params object[] args)
        {
            var formattedText = args.Length > 0 ?
                string.Format(text, args) :
                text;

            _callback.Invoke(new Message(formattedText, level));
        }
    }
}