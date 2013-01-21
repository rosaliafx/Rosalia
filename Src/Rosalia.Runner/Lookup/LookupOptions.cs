namespace Rosalia.Runner.Lookup
{
    public class LookupOptions
    {
        public LookupOptions(RunningOptions runningOptions)
        {
            RunningOptions = runningOptions;
        }

        public RunningOptions RunningOptions { get; private set; }
    }
}