namespace Rosalia.Runner.Lookup
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class ExactAssemblyWorkflowLookup : AbstractAssemblyWorkflowLookup
    {
        public override bool CanHandle(LookupOptions options)
        {
            return Path.GetExtension(options.RunningOptions.InputFile).Equals(".dll");
        }

        protected override IEnumerable<Assembly> GetAssemblies(LookupOptions options)
        {
            yield return Assembly.LoadFile(options.RunningOptions.InputFile);
        }
    }
}