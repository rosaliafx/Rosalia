namespace Rosalia.TaskLib.Standard.Tasks
{
    using Rosalia.Core.Fluent;

    public class SimpleExternalToolTask : ExternalToolTask<object>
    {
        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}