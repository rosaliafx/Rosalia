namespace Rosalia.TaskLib.Git.Input
{
    using Rosalia.TaskLib.Standard.Input;

    /// <summary>
    /// A base class for git tasks.
    /// </summary>
    public class GitInput : ExternalToolInput
    {
        public GitInput()
        {
        }

        public GitInput(string rawCommand)
        {
            RawCommand = rawCommand;
        }

        public string RawCommand { get; set; }

        public override string Arguments
        {
            get
            {
                return RawCommand;
            }

            set
            {
                RawCommand = value;
            }
        }
    }
}