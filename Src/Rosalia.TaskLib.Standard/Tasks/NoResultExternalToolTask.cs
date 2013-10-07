namespace Rosalia.TaskLib.Standard.Tasks
{
    using Rosalia.Core.Fluent;

    public abstract class NoResultExternalToolTask : ExternalToolTask<object>
    {
        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}