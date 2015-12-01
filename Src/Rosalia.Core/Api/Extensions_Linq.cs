namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    public static partial class Extensions
    {
        public static ITaskFuture<TResult> Select<TResult, TInput>(this ITaskFuture<TInput> input, Func<TInput, ITaskResult<TResult>> map)
            where TResult : class
            where TInput : class
        {
            return input.SelectCore((_, inputValue) => map(inputValue));
        }

        public static ITaskFuture<TResult> Select<TResult, TInput>(this ITaskFuture<TInput> input, Func<TInput, ITask<TResult>> map)
            where TResult : class
            where TInput : class
        {
            return input.SelectCore((context, inputValue) => map(inputValue).Execute(context));
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
            return input.SelectCore((context, inputValue) => context.ApplyActionAsTask(map.Invoke(inputValue)));
        }

        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, TResult> resultSelector) 
                where TResult : class
                where TInput : class 
                where TIntermediate : class
        {
            return input.SelectManyCore(
                func,
                (_, inputValue, temp) => resultSelector.Invoke(inputValue, temp).AsTaskResult());
        }

        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, ITask<TResult>> resultSelector) 
                where TResult : class 
                where TInput : class 
                where TIntermediate : class
        {
            return input.SelectManyCore(
                func,
                (context, inputValue, tempValue) => resultSelector.Invoke(inputValue, tempValue).Execute(context));
        }

        public static ITaskFuture<TResult> SelectMany<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TInput, TIntermediate, ITaskResult<TResult>> resultSelector) 
                where TResult : class 
                where TInput : class 
                where TIntermediate : class
        {
            return input.SelectManyCore(
                func,
                (_, inputValue, tempValue) => resultSelector.Invoke(inputValue, tempValue));
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
            return input.SelectManyCore(
                func,
                (context, inputValue, tempValue) => context.ApplyActionAsTask(resultSelector.Invoke(inputValue, tempValue)));
        }

        /// <summary>
        /// Core implementation of select.
        /// </summary>
        private static ITaskFuture<TResult> SelectCore<TResult, TInput>(this ITaskFuture<TInput> input, Func<TaskContext, TInput, ITaskResult<TResult>> map)
            where TResult : class
            where TInput : class
        {
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context => map.Invoke(context, input.FetchValue(context)));

            return new LinqTaskFuture<TResult>(delegatingTask, input);
        }

        /// <summary>
        /// Converts an action to a TaskResult of Nothing.
        /// </summary>
        private static ITaskResult<Nothing> ApplyActionAsTask(this TaskContext context, Action<TaskContext> action)
        {
            action.Invoke(context);
            return Nothing.Value.AsTaskResult();
        }

        private static ITaskFuture<TResult> SelectManyCore<TInput, TIntermediate, TResult>(
            this ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func,
            Func<TaskContext, TInput, TIntermediate, ITaskResult<TResult>> resultSelector)
            where TResult : class
            where TInput : class
            where TIntermediate : class
        {
            ITaskFuture<object>[] allDependencies = GetAllDependencies(input, func);
            ITask<TResult> delegatingTask = new FuncTask<TResult>(context =>
            {
                TInput inputValue = input.FetchValue(context);
                ITaskFuture<TIntermediate> intermediate = func.Invoke(inputValue);

                return resultSelector.Invoke(context, inputValue, intermediate.FetchValue(context));
            });

            return new LinqTaskFuture<TResult>(delegatingTask, allDependencies);
        }

        private static ITaskFuture<object>[] GetAllDependencies<TInput, TIntermediate>(
            ITaskFuture<TInput> input,
            Func<TInput, ITaskFuture<TIntermediate>> func)
            where TInput : class
            where TIntermediate : class
        {
            //// the func delegate is a function that returns the previous dependant task 
            //// and it normally does not use the input argument in any way:
            ////      SelectMany(valueFromTwoLevelsUpTask => levelOneUpTask, ...)
            //// So we can pass null (or default value) get a reference to the previous task.
            ITaskFuture<object> secondaryDependency = func.Invoke(default(TInput));

            ITaskFuture<object>[] recentDependencies = input is ILinqTaskFuture
                ? ((ILinqTaskFuture)input).Dependencies
                : new ITaskFuture<object>[] { input };

            return new List<ITaskFuture<object>>(recentDependencies)
            {
                secondaryDependency
            }.ToArray();
        }
    }

    /// <summary>
    /// Just a non-generic version of <code>LinqTaskFuture</code> class.
    /// The main purpose of this interface is to get dependencies out of LinqTaskFuture
    /// </summary>
    public interface ILinqTaskFuture
    {
        /// <summary>
        /// Gets a payload task of this task future.
        /// </summary>
        ITask<object> Task { get; }

        /// <summary>
        /// Gets all dependencies.
        /// </summary>
        ITaskFuture<object>[] Dependencies { get; }
    }

    /// <summary>
    /// A special kind of future used to register tasks via LINQ.
    /// </summary>
    public class LinqTaskFuture<TResult> : TaskFuture<TResult>, ILinqTaskFuture where TResult : class
    {
        private readonly ITask<TResult> _task;
        private readonly ITaskFuture<object>[] _dependencies;

        public LinqTaskFuture(ITask<TResult> task, params ITaskFuture<object>[] dependencies)
        {
            _task = task;
            _dependencies = dependencies;
        }

        public override Identity Identity
        {
            get
            {
                //// LinqTaskFuture never gets registered directly
                //// so we can return null as an Identity
                return null;
            }
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