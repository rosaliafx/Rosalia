namespace Rosalia.TestingSupport.Helpers
{
    using Rosalia.Core;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public static class SubflowExtensions
    {
        public static ITaskResult<T> Execute<T>(this TaskRegistry<T> subflow) where T : class
        {
            return new SubflowTask<T>(subflow, Identities.Empty).Execute();
        }
    }
}