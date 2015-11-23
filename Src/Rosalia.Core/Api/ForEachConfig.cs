namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;

    public class ForEachConfig<T>
    {
        private readonly IEnumerable<T> _items;

        public ForEachConfig(IEnumerable<T> items)
        {
            _items = items;
        }

        public ITaskRegistry<Nothing> Do(Func<T, ITask<Nothing>> task, Func<T, string> name)
        {
            return Do(task, _ => Nothing.Value, name);
        }

        public ITaskRegistry<TAggregatedResult> Do<TTaskResult, TAggregatedResult>(
            Func<T, ITask<TTaskResult>> task,
            Func<IEnumerable<TTaskResult>, TAggregatedResult> aggregator,
            Func<T, string> name) where TTaskResult : class where TAggregatedResult : class
        {
            return new RepeaterRegistry<T, TTaskResult, TAggregatedResult>(_items, task, aggregator, name);
        }
    }

    internal class RepeaterRegistry<TItem, TTaskResult, TAggregatedResult> : ITaskRegistry<TAggregatedResult> where TTaskResult : class where TAggregatedResult : class
    {
        private readonly IEnumerable<TItem> _source;
        private readonly Func<TItem, ITask<TTaskResult>> _task;
        private readonly Func<IEnumerable<TTaskResult>, TAggregatedResult> _aggregator;
        private readonly Func<TItem, string> _name;

        public RepeaterRegistry(
            IEnumerable<TItem> source, 
            Func<TItem, ITask<TTaskResult>> task, 
            Func<IEnumerable<TTaskResult>, 
            TAggregatedResult> aggregator, 
            Func<TItem, string> name)
        {
            _source = source;
            _task = task;
            _aggregator = aggregator;
            _name = name;
        }

        public IDirectory WorkDirectory
        {
            set {  }
        }

        public IEnvironment Environment
        {
            set {  }
        }

        public RegisteredTasks GetRegisteredTasks()
        {
            var map = new TaskMap();
            var taskDefinitions = new List<ITaskFuture<TTaskResult>>();

            foreach (TItem item in _source)
            {
                taskDefinitions.Add(map.Register(_name(item), _task(item), new ITaskBehavior[] {}));
            }

            var resultTask = map.Register(
                Guid.NewGuid().ToString(),
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