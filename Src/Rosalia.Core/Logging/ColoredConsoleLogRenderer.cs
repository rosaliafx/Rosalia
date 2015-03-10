namespace Rosalia.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class ColoredConsoleLogRenderer : ILogRenderer
    {
        private int _consoleWidth;
        private object _lockObject = new object();

        public void Init()
        {
            try
            {
                _consoleWidth = Console.WindowWidth;
            }
            catch (Exception)
            {
                // we are in the redirected output mode
                // let the console be as wide as possible
                _consoleWidth = 1000;
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
                case MessageLevel.Warn:
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

        private Identity _lastSource;

        public void Render(Message message, Identity source)
        {
            lock (_lockObject)
            {
                var level = message.Level;

                if (_lastSource == null || !Equals(_lastSource, source))
                {
                    // WriteColored(string.Format("[{0}] ", level), GetColorForLevel(level));
                    WriteColored(string.Format("[{0}]", source.Value), ConsoleColor.DarkMagenta);
                    Console.WriteLine();    
                }

                var lines = message.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                var procesedLines = new List<string>();
                var fullIndentSize = 8;
                var maxWidth = _consoleWidth - fullIndentSize - 1;
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
                    //Console.Write(new string(' ', fullIndentSize));

                    var levelString = " " + level.ToString().ToUpper();
                    WriteColored(levelString, ConsoleColor.DarkGray);
                    Console.Write(new string(' ', fullIndentSize - levelString.Length));

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

                _lastSource = source;
            }
        }
    }
}