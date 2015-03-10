namespace Rosalia.TaskLib.Svn
{
    public class SvnVersionResult
    {
        public SvnVersionResult(SvnVersion min, SvnVersion max)
        {
            Min = min;
            Max = max;
        }

        public SvnVersion Min { get; private set; }

        public SvnVersion Max { get; private set; }
    }
}