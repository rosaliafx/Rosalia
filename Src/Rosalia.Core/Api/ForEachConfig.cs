namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    public class ForEachConfig<T>
    {
        private readonly IEnumerable<T> _items;

        public ForEachConfig(IEnumerable<T> items)
        {
            _items = items;
        }

        public ITaskRegistry<Nothing> Do(Func<T, ITask<Nothing>> task)
        {
            return Do(task, _ => Nothing.Value);
        }

        public ITaskRegistry<TAggregatedResult> Do<TTaskResult, TAggregatedResult>(
            Func<T, ITask<TTaskResult>> task,
            Func<IEnumerable<TTaskResult>, TAggregatedResult> aggregator) where TTaskResult : class where TAggregatedResult : class
        {
            return new RepeaterRegistry<T, TTaskResult, TAggregatedResult>(_items, task, aggregator);
        }
    }

    internal class RepeaterRegistry<TItem, TTaskResult, TAggregatedResult> : ITaskRegistry<TAggregatedResult> where TTaskResult : class where TAggregatedResult : class
    {
        private readonly IEnumerable<TItem> _source;
        private readonly Func<TItem, ITask<TTaskResult>> _task;
        private readonly Func<IEnumerable<TTaskResult>, TAggregatedResult> _aggregator;

        public RepeaterRegistry(IEnumerable<TItem> source, Func<TItem, ITask<TTaskResult>> task, Func<IEnumerable<TTaskResult>, TAggregatedResult> aggregator)
        {
            _source = source;
            _task = task;
            _aggregator = aggregator;
        }

        public RegisteredTasks GetRegisteredTasks()
        {
            var map = new TaskMap();
            var index = 1;
            var taskDefinitions = new List<ITaskFuture<TTaskResult>>();

            foreach (TItem item in _source)
            {
                taskDefinitions.Add(map.Register(/* todo: check task name */ "task_" + index, _task.Invoke(item), new ITaskBehavior[] {}));
                index++;
            }

            var resultTask = map.Register(
                "result",
                new FuncTask<TAggregatedResult>(context =>
                {
                    var itemResults = taskDefinitions.Select(def => def.FetchValue(context)).ToList();
                    return _aggregator.Invoke(itemResults).AsTaskResult();
                }), 
                taskDefinitions.Select(def => new DependsOnBehavior(def.Identity)).Cast<ITaskBehavior>().ToArray());

            var resultTaskId = resultTask.Identity;

            return new RegisteredTasks(map.Map, resultTaskId, new Identities(resultTaskId));
        }
    }
}