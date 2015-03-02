namespace Rosalia.TaskLib.Standard.Tasks
{
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class ExecTask : ExternalToolTask<Nothing>
    {
        protected override Nothing CreateResult(int exitCode, TaskContext context)
        {
            return Nothing.Value;
        }
    }
}