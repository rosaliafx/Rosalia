namespace Rosalia.Runner.Console.CommandLine
{
    public static class ExitCode
    {
        public const int Ok = 0;

        public static class Error
        {
            public const int WorkflowFailure = -1;
            public const int RunnerInitializationFailure = -2;
            public const int OptionsParsingFailure = -3;
        }
    }
}