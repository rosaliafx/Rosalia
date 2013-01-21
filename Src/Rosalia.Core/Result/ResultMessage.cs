namespace Rosalia.Core.Result
{
    using Rosalia.Core.Logging;

    public class ResultMessage
    {
        public ResultMessage(MessageLevel level, string text)
        {
            Level = level;
            Text = text;
        }

        public MessageLevel Level { get; private set; }

        public string Text { get; private set; }
    }
}