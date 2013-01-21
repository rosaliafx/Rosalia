namespace Rosalia.Core.Tests.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Logging;

    public class LoggerStub : ILogger
    {
        private readonly IList<LogItem> _items = new List<LogItem>();

        public int MessagesCount
        {
            get
            {
                return _items.Count;
            }
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            _items.Add(new LogItem
            {
                Message = message,
                Level = level,
                Args = args
            });
        }

        public bool HasMessage(Func<MessageLevel, string, object[], bool> predicate)
        {
            return _items.Any(x => predicate(x.Level, x.Message, x.Args));
        }

        public bool HasError(Func<string, object[], bool> predicate)
        {
            return _items.Any(x => x.Level == MessageLevel.Error && predicate(x.Message, x.Args));
        }

        public bool HasInfo(Func<string, object[], bool> predicate)
        {
            return _items.Any(x => x.Level == MessageLevel.Info && predicate(x.Message, x.Args));
        }

        public class LogItem
        {
            public MessageLevel Level { get; set; }

            public string Message { get; set; }

            public object[] Args { get; set; }
        }

        public bool HasError()
        {
            return HasError((s, objects) => true);
        }
    }
}