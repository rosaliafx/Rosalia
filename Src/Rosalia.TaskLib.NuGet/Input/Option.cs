namespace Rosalia.TaskLib.NuGet.Input
{
    public class Option
    {
        public Option(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public string CommandLinePart
        {
            get
            {
                return string.IsNullOrEmpty(Value) ?
                    string.Format("-{0}", Name) :
                    string.Format("-{0} {1}", Name, Value);
            }
        }
    }
}