namespace Rosalia.TaskLib.MsBuild
{
    public class MsBuildSwitch
    {
        public static MsBuildSwitch Custom(string @switch, string parameter = null)
        {
            return new MsBuildSwitch(@switch, parameter);
        }

        public static MsBuildSwitch Configuration(string configuration)
        {
            return Property("Configuration", configuration);
        }

        public static MsBuildSwitch Property(string name, string value)
        {
            return Custom(PropertyKey, string.Format("{0}={1}", name, value));
        }

        public static MsBuildSwitch Targets(params string[] targets)
        {
            return Custom(TargetKey, string.Join(",", targets));
        }

        public static MsBuildSwitch Verbosity(string level)
        {
            return Custom(VerbosityKey, level);
        }

        public static MsBuildSwitch VerbosityQuiet()
        {
            return Verbosity("q");
        }

        public static MsBuildSwitch VerbosityMinimal()
        {
            return Verbosity("m");
        }

        public static MsBuildSwitch VerbosityNormal()
        {
            return Verbosity("n");
        }

        public static MsBuildSwitch VerbosityDetailed()
        {
            return Verbosity("d");
        }

        public static MsBuildSwitch VerbosityDiagnostic()
        {
            return Verbosity("diag");
        }

        public const string TargetKey = "target";
        public const string PropertyKey = "property";
        public const string VerbosityKey = "verbosity";

        private MsBuildSwitch(string text, string parameter)
        {
            Text = text;
            Parameter = parameter;
        }

        public string Text { get; private set; }

        public string Parameter { get; private set; }

        public string CommandLinePart
        {
            get
            {
                return string.IsNullOrEmpty(Parameter) ?
                    string.Format("/{0}", Text) :
                    string.Format("/{0}:{1}", Text, Parameter);
            }
        }
    }
}