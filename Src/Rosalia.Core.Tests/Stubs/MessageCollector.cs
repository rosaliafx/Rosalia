namespace Rosalia.Core.Tests.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Rosalia.Core.Logging;

    public class MessageCollector
    {
        private readonly IList<Message> _items = new List<Message>();


        public int MessagesCount
        {
            get
            {
                return _items.Count;
            }
        }

        public Message LastError
        {
            get { return GetLastMessage(MessageLevel.Error); }
        }

        public Message LastWarning
        {
            get { return GetLastMessage(MessageLevel.Warning); }
        }

        public Message LastInfo
        {
            get { return GetLastMessage(MessageLevel.Info); }
        }

        public void RegisterMessage(Message message)
        {
            _items.Add(message);
        }

        public Message GetLastMessage(MessageLevel level)
        {
            return _items.Reverse().FirstOrDefault(x => x.Level == level);
        }

        public void AssertHasMessage(MessageLevel level)
        {
            var result = GetLastMessage(level);

            if (result == null)
            {
                Assert.Fail("Expected message of level {0} was not registered", level);
            }
        }

        public void AssertHasWarning()
        {
            AssertHasMessage(MessageLevel.Warning);
        }

        public void AssertHasInfo()
        {
            AssertHasMessage(MessageLevel.Info);
        }

        public void AssertHasError()
        {
            AssertHasMessage(MessageLevel.Error);
        }
    }
}