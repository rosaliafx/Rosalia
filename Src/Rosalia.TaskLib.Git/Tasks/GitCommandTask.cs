namespace Rosalia.TaskLib.Git.Tasks
{
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Git.Input;

    public class GitCommandTask<T> : AbstractGitTask<T, GitInput, object>
    {
        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}