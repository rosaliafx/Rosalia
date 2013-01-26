namespace Rosalia.Core.Logging
{
    public class Message
    {
        public Message(string text, MessageLevel level)
        {
            Text = text;
            Level = level;
        }

        public string Text { get; private set; }

        public MessageLevel Level { get; private set; }
    }
}