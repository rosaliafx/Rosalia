namespace Rosalia.Core.Logging
{
    public class Message
    {
        private readonly string _text;
        private readonly MessageLevel _level;

        public Message(string text, MessageLevel level)
        {
            _text = text;
            _level = level;
        }

        public string Text
        {
            get { return _text; }
        }

        public MessageLevel Level
        {
            get { return _level; }
        }
    }
}