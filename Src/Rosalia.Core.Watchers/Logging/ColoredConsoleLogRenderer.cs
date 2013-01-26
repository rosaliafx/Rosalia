namespace Rosalia.Core.Watchers.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Logging;

    public class ColoredConsoleLogRenderer : ILogRenderer
    {
        public void Init()
        {
        }

        public void AppendMessage(int depth, string message, MessageLevel level, MessageType type)
        {
            var indentSize = depth * 2;
            var prefix = string.Format("[{0}] ", level);

            Console.Write(new string(' ', indentSize));
            WriteColored(prefix, ConsoleColor.DarkGray);

            var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            var fullIndentSize = indentSize + prefix.Length;
            var maxWidth = Console.WindowWidth - fullIndentSize - 1;

            var procesedLines = new List<string>();
            foreach (var line in lines)
            {
                var currentLine = line;
                while (currentLine.Length > maxWidth)
                {
                    procesedLines.Add(currentLine.Substring(0, maxWidth));
                    currentLine = currentLine.Substring(maxWidth);
                }

                procesedLines.Add(currentLine);
            }

            if (procesedLines.Count > 0)
            {
                WriteColored(procesedLines[0], GetColorForLevel(level));    
                Console.WriteLine();

                if (procesedLines.Count > 1)
                {
                    for (int i = 1; i < procesedLines.Count; i++)
                    {
                        Console.Write(new string(' ', fullIndentSize));
                        WriteColored(procesedLines[i], GetColorForLevel(level));
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine();
            }
        }

        private ConsoleColor GetColorForLevel(MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Info:
                    return ConsoleColor.Gray;
                case MessageLevel.Success:
                    return ConsoleColor.DarkGreen;
                case MessageLevel.Warning:
                    return ConsoleColor.DarkYellow;
                case MessageLevel.Error:
                    return ConsoleColor.DarkRed;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }

        private void WriteColored(string text, ConsoleColor consoleColor)
        {
            var initialColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.Write(text);
            Console.ForegroundColor = initialColor;
        }

        public void Dispose()
        {
        }
    }
}