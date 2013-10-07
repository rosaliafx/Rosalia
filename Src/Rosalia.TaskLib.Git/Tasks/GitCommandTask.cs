namespace Rosalia.TaskLib.Git.Tasks
{
    using Rosalia.Core.Fluent;

    public class GitCommandTask : AbstractGitTask<object>
    {
        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}