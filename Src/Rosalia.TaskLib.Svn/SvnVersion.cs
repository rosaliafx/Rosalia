namespace Rosalia.TaskLib.Svn
{
    public class SvnVersion
    {
        public SvnVersion(int number, string trail)
        {
            Number = number;
            Trail = trail;
        }

        /// <summary>
        /// Gets a revision number.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Gets a revision trailing string value. Could contain 'M', 'S', and 'P' chars.
        /// </summary>
        public string Trail { get; private set; }
    }
}