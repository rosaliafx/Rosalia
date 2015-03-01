using Rosalia.Core.Common;

namespace Rosalia.Core.Engine
{
    public class TaskInfo
    {
        private readonly Identity _identity;
        private readonly IFlowable<object> _flowable;
        private readonly Identity[] _dependencies;

        public TaskInfo(Identity identity, IFlowable<object> flowable, Identity[] dependencies)
        {
            _identity = identity;
            _flowable = flowable;
            _dependencies = dependencies;
        }

        public Identity Identity
        {
            get { return _identity; }
        }
    }
}