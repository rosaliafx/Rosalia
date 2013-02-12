namespace Rosalia.TaskLib.MsBuild
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.FileSystem;
    using Rosalia.TaskLib.Standard.Input;

    public class MsBuildInput : ExternalToolInput
    {
        private readonly IList<MsBuildSwitch> _switches = new List<MsBuildSwitch>();

        public MsBuildInput()
        {
            ToolVersion = MsBuildToolVersion.V40;
        }

        public IFile ProjectFile { get; set; }

        public MsBuildToolVersion ToolVersion { get; set; }

        public IEnumerable<MsBuildSwitch> Switches
        {
            get { return _switches; }
        }

        public MsBuildInput WithProjectFile(IFile projectFile)
        {
            ProjectFile = projectFile;
            return this;
        }

        public MsBuildInput WithProjectFile(string projectFile)
        {
            ProjectFile = new DefaultFile(projectFile);
            return this;
        }

        public MsBuildInput WithSwitch(string @switch)
        {
            _switches.Add(new MsBuildSwitch(@switch, null));
            return this;
        }

        public MsBuildInput WithSwitch(string @switch, string parameter)
        {
            _switches.Add(new MsBuildSwitch(@switch, parameter));
            return this;
        }

        public MsBuildInput WithConfiguration(string configuration)
        {
            return WithProperty("Configuration", configuration);
        }

        public MsBuildInput WithProperty(string name, string value)
        {
            return WithSwitch(MsBuildSwitch.Property, string.Format("{0}={1}", name, value));
        }

        public MsBuildInput WithBuildTargets(params string[] targets)
        {
            var switchesToRemove = _switches.Where(x => x.Text == MsBuildSwitch.Target).ToArray();
            foreach (var @switch in switchesToRemove)
            {
                _switches.Remove(@switch);
            }

            return WithSwitch(MsBuildSwitch.Target, string.Join(",", targets));
        }

        public MsBuildInput WithVerbosity(string level)
        {
            return WithSwitch(MsBuildSwitch.Verbosity, level);
        }

        public MsBuildInput WithVerbosityQuiet()
        {
            return WithVerbosity("q");
        }

        public MsBuildInput WithVerbosityMinimal()
        {
            return WithVerbosity("m");
        }

        public MsBuildInput WithVerbosityNormal()
        {
            return WithVerbosity("n");
        }

        public MsBuildInput WithVerbosityDetailed()
        {
            return WithVerbosity("d");
        }

        public MsBuildInput WithVerbosityDiagnostic()
        {
            return WithVerbosity("diag");
        }
    }
}