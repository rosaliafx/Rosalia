namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    public static class Linq
    {
        public static ITask<T> ToTask<T>(this ITaskRegistry<T> registry) where T : class
        {
            return new SubflowTask<T>(registry, Identities.Empty);
        }

        public static ITask<TResult> AsExecutable<TResult>(this ITask<TResult> task) where TResult : class
        {
            return task;
        }

        public static ITask<TResult> WithPrecondition<TResult>(this ITask<TResult> task, Func<bool> predicate, Func<TResult> defaultValueProvider) where TResult : class
        {
            return new PreconditionTask<TResult>(task, predicate, defaultValueProvider);
        }

        public static ITask<TResult> WithPrecondition<TResult>(this ITask<TResult> task, Func<bool> predicate, TResult defaultValue) where TResult : class
        {
            return new PreconditionTask<TResult>(task, predicate, defaultValue);
        }

        public static ITask<TResult> WithPrecondition<TResult>(this ITask<TResult> task, Func<bool> predicate) where TResult : class
        {
            return new PreconditionTask<TResult>(task, predicate);
        }

        public static ITask<TResult> WithPrecondition<TResult>(this ITask<TResult> task, bool predicate, Func<TResult> defaultValueProvider) where TResult : class
        {
            return new PreconditionTask<TResult>(task, () => predicate, defaultValueProvider);
        }

        public static ITask<TResult> WithPrecondition<TResult>(this ITask<TResult> task, bool predicate, TResult defaultValue) where TResult : class
        {
            return new PreconditionTask<TResult>(task, () => predicate, defaultValue);
        }

        public static ITask<TResult> WithPrecondition<TResult>(this ITask<TResult> task, bool predicate) where TResult : class
        {
            return new PreconditionTask<TResult>(task, () => predicate);
        }

        public static ITask<TResult> RecoverWith<TResult>(this ITask<TResult> task, Func<TResult> valueProvider) where TResult : class
        {
            return new RecoverTask<TResult>(task, valueProvider);
        }

        public static ITask<TResult> RecoverWith<TResult>(this ITask<TResult> task, TResult value) where TResult : class
        {
            return new RecoverTask<TResult>(task, value);
        }

        public static ITaskFuture<TResult> Select<TResult, TInput>(this ITaskFuture<TInput> input, Func<TInput, ITask<TResult>> map)
            where TResult : class
            where TInput : class
        {
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context => map.Invoke(input.FetchValue(context)).Execute(context));

            return new LinqTaskFuture<TResult>(delegatingTask, new ITaskFuture<object>[] { input });
        }

        public static ITaskFuture<TResult> Select<TResult, TInput>(this ITaskFuture<TInput> input, Func<TInput, ITaskResult<TResult>> map)
            where TResult : class
            where TInput : class
        {
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context => map.Invoke(input.FetchValue(context)));

            return new LinqTaskFuture<TResult>(delegatingTask, new ITaskFuture<object>[] { input });
        }

        public static ITaskFuture<TResult> Select<TResult, TInput>(this ITaskFuture<TInput> input, Func<TInput, ITaskRegistry<TResult>> map)
            where TResult : class
            where TInput : class
        {
            return Select(input, inputArg => map.Invoke(inputArg).ToTask());
        }

        public static ITaskFuture<Nothing> Select<TInput>(this ITaskFuture<TInput> input, Func<TInput, Action<TaskContext>> map)
            where TInput : class
        {
            ITask<Nothing> delegatingTask = new FuncTask<Nothing>(context =>
            {
                map.Invoke(input.FetchValue(context));
                return Nothing.Value.AsTaskResult();
            });

            return new LinqTaskFuture<Nothing>(delegatingTask, new ITaskFuture<object>[] { input });
        }
        
        
        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, TResult> resultSelector) 
                where TResult : class
                where TInput : class 
                where TIntermediate : class
        {
            ITaskFuture<object>[] allDependencies = GetAllDependencies(input, func);
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context =>
            {
                TInput inputValue = input.FetchValue(context);
                ITaskFuture<TIntermediate> intermediate = func.Invoke(inputValue);
                
                return new SuccessResult<TResult>(resultSelector.Invoke(inputValue, intermediate.FetchValue(context))); //.Execute(context);
            });

            return new LinqTaskFuture<TResult>(delegatingTask, allDependencies);
        }

        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, ITask<TResult>> resultSelector) 
                where TResult : class 
                where TInput : class 
                where TIntermediate : class
        {
            ITaskFuture<object>[] allDependencies = GetAllDependencies(input, func);
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context =>
            {
                TInput inputValue = input.FetchValue(context);
                ITaskFuture<TIntermediate> intermediate = func.Invoke(inputValue);
                
                return resultSelector.Invoke(inputValue, intermediate.FetchValue(context)).Execute(context);
            });

            return new LinqTaskFuture<TResult>(delegatingTask, allDependencies);
        }

        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, ITaskResult<TResult>> resultSelector) 
                where TResult : class 
                where TInput : class 
                where TIntermediate : class
        {
            ITaskFuture<object>[] allDependencies = GetAllDependencies(input, func);
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context =>
            {
                TInput inputValue = input.FetchValue(context);
                ITaskFuture<TIntermediate> intermediate = func.Invoke(inputValue);
                
                return resultSelector.Invoke(inputValue, intermediate.FetchValue(context));
            });

            return new LinqTaskFuture<TResult>(delegatingTask, allDependencies);
        }

        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, ITaskRegistry<TResult>> resultSelector) 
                where TResult : class 
                where TInput : class 
                where TIntermediate : class
        {
            return SelectMany(input, func, (inputArg, intermediaArg) => resultSelector.Invoke(inputArg, intermediaArg).ToTask());
        }

        public static ITaskFuture<Nothing> SelectMany<TInput, TIntermediate>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, Action<TaskContext>> resultSelector) 
                where TInput : class 
                where TIntermediate : class
        {
            ITaskFuture<object>[] allDependencies = GetAllDependencies(input, func);
            ITask<Nothing> delegatingTask = new FuncTask<Nothing>(context =>
            {
                TInput inputValue = input.FetchValue(context);
                ITaskFuture<TIntermediate> intermediate = func.Invoke(inputValue);
                
                resultSelector.Invoke(inputValue, intermediate.FetchValue(context)).Invoke(context);

                return Nothing.Value.AsTaskResult();
            });

            return new LinqTaskFuture<Nothing>(delegatingTask, allDependencies);
        }

        private static ITaskFuture<object>[] GetAllDependencies<TInput, TIntermediate>(
            ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func)
            where TInput : class
            where TIntermediate : class
        {
            var secondaryDependency = (ITaskFuture<object>)func.Invoke(default(TInput));
            var recentDependencies = input is ILinqTaskFuture
                ? ((ILinqTaskFuture)input).Dependencies
                : new ITaskFuture<object>[] { input };

            return new List<ITaskFuture<object>>(recentDependencies)
            {
                secondaryDependency
            }.ToArray();
        }
    }

    public interface ILinqTaskFuture
    {
        Identity Identity { get; }

        ITask<object> Task { get; }

        ITaskFuture<object>[] Dependencies { get; }
    }

    public class LinqTaskFuture<TResult> : TaskFuture<TResult>, ILinqTaskFuture where TResult : class
    {
        private readonly ITask<TResult> _task;
        private readonly ITaskFuture<object>[] _dependencies;
        private Identity _identity;

        public LinqTaskFuture(ITask<TResult> task, ITaskFuture<object>[] dependencies)
        {
            _task = task;
            _dependencies = dependencies;
        }

        public override Identity Identity
        {
            get { return _identity; }
        }

        public ITask<object> Task
        {
            get { return _task; }
        }

        public ITaskFuture<object>[] Dependencies
        {
            get { return _dependencies; }
        }

        public override TResult FetchValue(TaskContext context)
        {
            return _task.Execute(context).Data;
        }
    }
}