namespace Rosalia.TestingSupport
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core;
    using Rosalia.Core.Logging;

    public class SpyLogRenderer : ILogRenderer
    {
        private readonly IList<Tuple<Message, Identity>> _messages = new List<Tuple<Message, Identity>>();

        public IList<Tuple<Message, Identity>> Messages
        {
            get { return _messages; }
        }

        public void Render(Message message, Identity source)
        {
            Messages.Add(new Tuple<Message, Identity>(message, source));
        }
    }
}