using NUnit.Framework;

namespace Rosalia.TestingSupport.Helpers
{
    using Rosalia.Core.Tasks.Results;

    public static class TaskResultExtensions
    {
        public static ITaskResult<T> AssertSuccess<T>(this ITaskResult<T> result)
        {
            if (!result.IsSuccess)
            {
                Assert.Fail("Expected success result but was error {0}", result.Error);
            }

            return result;
        }

        public static ITaskResult<T> AssertDataIs<T>(this ITaskResult<T> result, T expected)
        {
            result.AssertSuccess();
            Assert.That(result.Data, Is.EqualTo(expected));

            return result;
        }
    }
}