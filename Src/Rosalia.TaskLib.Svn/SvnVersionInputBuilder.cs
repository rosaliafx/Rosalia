namespace Rosalia.TaskLib.Svn
{
    public class SvnVersionInputBuilder
    {
        private SvnVersionInput _result;

        public SvnVersionInputBuilder SvnExePath(string path)
        {
            _result = new SvnVersionInput(path);
            return this;
        }

        public static implicit operator SvnVersionInput(SvnVersionInputBuilder builder)
        {
            return builder.ToResult();
        }

        private SvnVersionInput ToResult()
        {
            return _result;
        }
    }
}