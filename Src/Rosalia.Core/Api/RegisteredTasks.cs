namespace Rosalia.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Engine.Composing;

    public class RegisteredTasks
    {
        private readonly IDictionary<Identity, FlowableWithDependencies> _map;
        private readonly Identity _resultTaskId;
        private readonly Identities _startupTaskIds;

        public RegisteredTasks(IDictionary<Identity, FlowableWithDependencies> map, Identity resultTaskId, Identities startupTaskIds)
        {
            _map = map;
            _resultTaskId = resultTaskId;
            _startupTaskIds = startupTaskIds;
        }

        public FlowableWithDependencies this[Identity identity]
        {
            get { return _map[identity]; }
        }

        public Identity ResultTaskId
        {
            get { return _resultTaskId; }
        }

        public Identities StartupTaskIds
        {
            get { return _startupTaskIds; }
        }

        public IEnumerable<T> Transform<T>(Func<Identity, FlowableWithDependencies, T> selector)
        {
            return _map.Select(pair => selector(pair.Key, pair.Value));
        }

        public IEnumerable<T> Transform<T>(Identities filter, Func<Identity, FlowableWithDependencies, T> selector)
        {
            return _map.Where(pair => filter.Contains(pair.Key)).Select(pair => selector(pair.Key, pair.Value));
        }
    }
}