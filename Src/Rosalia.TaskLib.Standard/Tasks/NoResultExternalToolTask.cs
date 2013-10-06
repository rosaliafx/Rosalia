namespace Rosalia.TaskLib.Standard.Tasks
{
    using Rosalia.Core.Fluent;

    public abstract class NoResultExternalToolTask<T> : ExternalToolTask<T, object>
    {
        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}