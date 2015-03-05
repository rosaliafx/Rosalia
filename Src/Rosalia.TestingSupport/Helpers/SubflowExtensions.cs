namespace Rosalia.TestingSupport.Helpers
{
    using Rosalia.Core;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;

    public static class SubflowExtensions
    {
        public static ResultWrapper<T> Execute<T>(this TaskRegistry<T> subflow) where T : class
        {
            return TaskExtensions.Execute(new SubflowTask<T>(subflow, Identities.Empty));
        }
    }
}