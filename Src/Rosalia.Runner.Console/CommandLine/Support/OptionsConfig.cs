namespace Rosalia.Runner.Console.CommandLine.Support
{
    using System.Collections.Generic;

    public class OptionsConfig : Dictionary<Option, System.Action<string, string>>
    {
    }
}