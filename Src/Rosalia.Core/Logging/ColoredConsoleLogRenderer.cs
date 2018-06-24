namespace Rosalia.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ColoredConsoleLogRenderer : ILogRenderer
    {
        private readonly object _lockObject = new object();
        private int _consoleWidth;
        private Identities _lastSource;

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

            Console.WriteLine("Console width:");
            Console.WriteLine(_consoleWidth);
            Console.WriteLine();
        }

        public void Dispose()
        {
        }

        public void Render(Message message, Identities source)
        {
            lock (_lockObject)
            {
                var level = message.Level;

                if (_lastSource == null || !Equals(_lastSource, source))
                {
                    for (int index = 0; index < source.Items.Length; index++)
                    {
                        Identity identity = source.Items[index];

                        WriteColored("[", ConsoleColor.DarkMagenta);
                        WriteColored(identity.Value, ConsoleColor.DarkMagenta);
                        WriteColored("]", ConsoleColor.DarkMagenta);

                        if (index < source.Items.Length - 1)
                        {
                            WriteColored("->", ConsoleColor.DarkCyan);    
                        }
                    }

                    Console.WriteLine();
                }

                var lines = message.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                var procesedLines = new List<string>();
                
                const int fullIndentSize = 8;

                var maxWidth = _consoleWidth - fullIndentSize - 1;
                if (maxWidth > 0)
                {
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
                }

                if (procesedLines.Count > 0)
                {
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
    }
}