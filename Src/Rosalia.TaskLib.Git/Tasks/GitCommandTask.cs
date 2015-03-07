namespace Rosalia.TaskLib.Git.Tasks
{
    using System.Collections.Generic;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class GitCommandTask : AbstractGitTask<Nothing>
    {
        protected override Nothing CreateResult(int exitCode, TaskContext context, IList<Message> aggregatedOutput)
        {
            return Nothing.Value;
        }
    }
}