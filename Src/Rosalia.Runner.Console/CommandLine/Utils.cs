namespace Rosalia.Runner.Console.CommandLine
{
    using System;
    using System.Reflection;
    using Rosalia.Runner.Console.CommandLine.Support;

    public static class Utils
    {
        public static void ShowLogo()
        {
            Console.WriteLine(
@"
  _____                 _ _       
 |  __ \               | (_)      
 | |__) |___  ___  __ _| |_  __ _ 
 |  _  // _ \/ __|/ _` | | |/ _` |
 | | \ \ (_) \__ \ (_| | | | (_| |
 |_|  \_\___/|___/\__,_|_|_|\__,_|
");
            Console.WriteLine(" v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine();
        }

        public static void ShowHelp(OptionsConfig optionsConfig)
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine();
            Console.WriteLine("        Rosalia.exe [OPTIONS] WORKFLOW_DLL");
            Console.WriteLine("   or   Rosalia.exe [OPTIONS] WORKFLOW_PROJECT");
            Console.WriteLine();
            Console.WriteLine("where");
            Console.WriteLine();
            Console.WriteLine("    WORKFLOW_DLL       Absolute or relative path to a workflow dll file");
            Console.WriteLine("    WORKFLOW_PROJECT   Absolute or relative path to a workflow csprj file");
            Console.WriteLine();
            Console.WriteLine("and options include");
            Console.WriteLine();

            foreach (var option in optionsConfig)
            {
                Console.WriteLine("    " + option.Key.Description);
                foreach (var aliase in option.Key.Aliases)
                {
                    Console.WriteLine("        /{0}", aliase);
                }

                Console.WriteLine();
            }
        }
    }
}