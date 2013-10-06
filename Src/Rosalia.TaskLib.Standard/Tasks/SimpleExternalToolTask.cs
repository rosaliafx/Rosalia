namespace Rosalia.TaskLib.Standard.Tasks
{
    using Rosalia.Core.Fluent;

    public class SimpleExternalToolTask<TContext> : ExternalToolTask<TContext, object>
    {
        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}