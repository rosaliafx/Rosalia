namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    public class AbstractTasksRegistry : TaskRegistryApiHelper, ITaskRegistryApi
    {
        private readonly TaskMap _taskMap = new TaskMap();
        private Identity _lastRegisteredTask = null;

        protected TaskMap TaskMap
        {
            get { return _taskMap; }
        }

        protected virtual bool IsSequence
        {
            get { return false; }
        }

        protected virtual DefaultTaskDetection DefaultTaskDetection
        {
            get { return DefaultTaskDetection.LastTask; }
        }

        public ITaskFuture<T> Task<T>(
            string id,
            ITask<T> task,
            params ITaskBehavior[] behaviors) where T : class
        {
            return DoRegister(id, task, behaviors);
        }

        public ITaskFuture<T> Task<T>(
            string id,
            Func<TaskContext, ITaskResult<T>> task,
            params ITaskBehavior[] behaviors) where T : class
        {
            return Task(id, new FuncTask<T>(task), behaviors);
        }

        public ITaskFuture<T> Task<T>(
            string id,
            Func<ITaskResult<T>> task,
            params ITaskBehavior[] behaviors) where T : class
        {
            return Task(id, _ => task.Invoke(), behaviors);
        }

        public ITaskFuture<T> Task<T>(
            string id,
            Func<TaskContext, ITask<T>> task,
            params ITaskBehavior[] behaviors) where T : class
        {
            return Task(id, new FuncTask<T>(context => task.Invoke(context).Execute(context)), behaviors);
        }

        public ITaskFuture<T> Task<T>(
            string id,
            ITaskFuture<T> task,
            params ITaskBehavior[] behaviors) where T : class
        {
            //            ILinqTaskFuture linqTask = (ILinqTaskFuture)task;
            //            List<ITaskFuture<object>> allDependencies = new List<ITaskFuture<object>>(linqTask.Dependencies);
            //
            //            allDependencies.AddRange(dependencies);
            //
            //            return TaskMap.Register(id, (ITask<T>)linqTask.task, allDependencies.ToArray());

            ILinqTaskFuture linqTask = (ILinqTaskFuture)task;

            List<ITaskBehavior> allDependencies = new List<ITaskBehavior>();
            allDependencies.AddRange(linqTask.Dependencies.Select(t => new DependsOnBehavior(t.Identity)));
            allDependencies.AddRange(behaviors);

            return DoRegister(id, (ITask<T>)linqTask.Task, allDependencies.ToArray());
            //return TaskMap.Register(id, (ITask<T>)linqTask.Task, allDependencies.ToArray());
        }

        public ITaskFuture<Nothing> Task(
            string id,
            Action<TaskContext> task,
            params ITaskBehavior[] behaviors)
        {
            return Task(
                id,
                context =>
                {
                    task.Invoke(context);
                    return Nothing.AsTaskResult();
                },
                behaviors);
        }

        public ITaskFuture<Nothing> Task(
            string id,
            Action task,
            params ITaskBehavior[] behaviors)
        {
            return Task(id, _ => task.Invoke(), behaviors);
        }

        public ITaskFuture<T> Task<T>(string id, ITaskRegistry<T> task, params ITaskBehavior[] behaviors) where T : class
        {
            return Task(id, task.ToTask(), behaviors);
        }

        public ITaskFuture<T> Task<T>(string id, Func<TaskContext, ITaskRegistry<T>> task, params ITaskBehavior[] behaviors) where T : class
        {
            return Task(id, context => task.Invoke(context).ToTask(), behaviors);
        }

        protected Identities GetDefaultTasks()
        {
            var tasksWithDefaultBehavior = TaskMap.DefaultTasks();
            if (!tasksWithDefaultBehavior.IsEmpty)
            {
                return tasksWithDefaultBehavior;
            }

            switch (DefaultTaskDetection)
            {
                case DefaultTaskDetection.LastTask:
                    return new Identities(_lastRegisteredTask);
                case DefaultTaskDetection.AllTasks:
                    return new Identities(TaskMap.Map.Keys.ToArray());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ITaskFuture<T> DoRegister<T>(string id, ITask<T> task, ITaskBehavior[] behaviors) where T : class
        {
            if (IsSequence && _lastRegisteredTask != null)
            {
                behaviors = new List<ITaskBehavior>(behaviors)
                {
                    DependsOn(_lastRegisteredTask)
                }.ToArray();
            }

            var registeredTask = TaskMap.Register(id, task, behaviors);

            _lastRegisteredTask = registeredTask.Identity;    

            return registeredTask;
        }
    }
}