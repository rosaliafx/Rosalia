﻿namespace Rosalia.Core.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;

    public class TaskMap
    {
        private readonly IDictionary<Identity, FlowableWithDependencies> _map = new Dictionary<Identity, FlowableWithDependencies>();

        public IDictionary<Identity, FlowableWithDependencies> Map
        {
            get { return _map; }
        }

        public RegistrationTaskFuture<T> Register<T>(string id, ITask<T> task, ITaskBehavior[] behaviors) where T : class
        {
            Identity identity = CreateIdentity(id);

            this[identity] = new FlowableWithDependencies(task, behaviors);

            return new RegistrationTaskFuture<T>(identity);
        }

        public FlowableWithDependencies this[Identity identity]
        {
            get { return Map[identity]; }
            set { Map[identity] = value; }
        }

        private static Identity CreateIdentity(string id)
        {
            return string.IsNullOrEmpty(id) ? new Identity() : new Identity(id);
        }

//        private static Identities GetIdentities(ITaskBehavior[] behaviors)
//        {
//            return new Identities(behaviors
//                .OfType<DependsOnBehavior>()
//                .Select(task => task.Identity).ToArray());
//        }

        public Identities DefaultTasks()
        {
            return new Identities(_map
                .Where(pair => pair.Value.IsDefault)
                .Select(pair => pair.Key)
                .ToArray());
        }
    }
}