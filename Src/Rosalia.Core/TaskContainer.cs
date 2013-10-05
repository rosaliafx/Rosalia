namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Tasks;

    [Obsolete]
    public class TaskContainer<TData>
    {
        private readonly IList<ITask<TData>> _tasks;

        private string _currentTaskName;

        public TaskContainer() : this(new List<ITask<TData>>())
        {
        }

        public TaskContainer(IList<ITask<TData>> tasks)
        {
            _tasks = tasks;
        }

        public IList<ITask<TData>> ConfiguredTasks
        {
            get { return _tasks; }
        }

        public string Name
        {
            set { _currentTaskName = value; }
        }

        public ITask<TData> Task
        {
            set
            {
                var currentTask = value;
                currentTask.Name = _currentTaskName;

                ConfiguredTasks.Add(currentTask);

                _currentTaskName = null;
            }
        }

        protected Action<ResultBuilder, TaskContext<TData>> TaskAction
        {
            set
            {
                Task = new SimpleTask<TData>(value);
            }
        }
    }
}