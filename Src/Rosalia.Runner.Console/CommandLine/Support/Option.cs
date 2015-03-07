namespace Rosalia.Runner.Console.CommandLine.Support
{
    public class Option
    {
        public Option(string aliases) : this(aliases, null)
        {
        }

        public Option(string aliases, string description)
        {
            Description = description;
            Aliases = aliases.Split('|');
        }

        public string[] Aliases { get; set; }

        public string Description { get; set; }
    }
}