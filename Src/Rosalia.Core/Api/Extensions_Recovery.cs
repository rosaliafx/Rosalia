namespace Rosalia.Core.Api
{
    using System;
    using Rosalia.Core.Tasks;

    public static partial class Extensions
    {
        public static ITask<TResult> RecoverWith<TResult>(this ITask<TResult> task, Func<TResult> valueProvider) where TResult : class
        {
            return new RecoverTask<TResult>(task, valueProvider);
        }

        public static ITask<TResult> RecoverWith<TResult>(this ITask<TResult> task, TResult value) where TResult : class
        {
            return new RecoverTask<TResult>(task, value);
        }
    }
}