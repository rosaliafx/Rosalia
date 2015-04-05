namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;

    public class TaskMap
    {
        private readonly IDictionary<Identity, TaskWithBehaviors> _map = new Dictionary<Identity, TaskWithBehaviors>();

        public IDictionary<Identity, TaskWithBehaviors> Map
        {
            get { return _map; }
        }

        public RegistrationTaskFuture<T> Register<T>(string id, ITask<T> task, ITaskBehavior[] behaviors) where T : class
        {
            Identity identity = CreateIdentity(id);

            if (Map.ContainsKey(identity))
            {
                throw new Exception(string.Format("Task with id [{0}] has already been registered", identity.Value));
            }

            this[identity] = new TaskWithBehaviors(task, behaviors);

            return new RegistrationTaskFuture<T>(identity);
        }

        public TaskWithBehaviors this[Identity identity]
        {
            get { return Map[identity]; }
            private set { Map[identity] = value; }
        }

        private static Identity CreateIdentity(string id)
        {
            return string.IsNullOrEmpty(id) ? new Identity() : new Identity(id);
        }

        public Identities DefaultTasks()
        {
            return new Identities(_map
                .Where(pair => pair.Value.IsDefault)
                .Select(pair => pair.Key)
                .ToArray());
        }
    }
}