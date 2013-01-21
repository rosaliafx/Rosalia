namespace Rosalia.TaskLib.MsBuild
{
    public class MsBuildToolVersion
    {
        public static readonly MsBuildToolVersion V20 = new MsBuildToolVersion("2.0");
        public static readonly MsBuildToolVersion V35 = new MsBuildToolVersion("3.5");
        public static readonly MsBuildToolVersion V40 = new MsBuildToolVersion("4.0");

        private readonly string _value;

        public MsBuildToolVersion(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}