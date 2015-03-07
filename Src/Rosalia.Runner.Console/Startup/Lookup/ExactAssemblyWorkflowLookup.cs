namespace Rosalia.Runner.Console.Startup.Lookup
{
    using System.Collections.Generic;
    using System.Reflection;

    public class ExactAssemblyWorkflowLookup : AbstractAssemblyWorkflowLookup
    {
        public override bool CanHandle(LookupOptions options)
        {
            return options.RunningOptions.InputFile.Extension.Is(".dll");
            //return Path.GetExtension(options.RunningOptions.InputFile).Equals(".dll");
        }

        protected override IEnumerable<Assembly> GetAssemblies(LookupOptions options)
        {
            yield return Assembly.LoadFile(options.RunningOptions.InputFile.AbsolutePath);
        }
    }
}