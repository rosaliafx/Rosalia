namespace Rosalia.TaskLib.MsBuild
{
    public class MsBuildSwitch
    {
        public const string Target = "target";
        public const string Property = "property";
        public const string Verbosity = "verbosity";

        public MsBuildSwitch(string text, string parameter)
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