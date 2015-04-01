namespace Rosalia.Core.Api
{
    using System;
    using Rosalia.Core.Tasks;

    public partial class Extensions
    {
        public static ITask<TOutput> TransformWith<TOutput, TInput>(this ITask<TInput> task, Func<TInput, TOutput> transformer) 
            where TOutput : class 
            where TInput : class
        {
            return new TransformTask<TInput, TOutput>(task, transformer);
        }
    }
}