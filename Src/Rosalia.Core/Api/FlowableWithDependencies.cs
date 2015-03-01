namespace Rosalia.Core.Api
{
    using System.Linq;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Tasks;

    public class FlowableWithDependencies
    {
        private readonly ITask<object> _task;
        private readonly ITaskBehavior[] _behaviors;
        //private readonly Identities _dependencies;

        public FlowableWithDependencies(ITask<object> task, params ITaskBehavior[] behaviors)
        {
            _task = task;
            _behaviors = behaviors;
        }

        public ITask<object> Task
        {
            get { return _task; }
        }

        public bool IsDefault
        {
            get { return _behaviors.OfType<DefaultBehavior>().Any(); }
        }

        public Identities Dependencies
        {
            get
            {
                return new Identities(_behaviors
                  .OfType<DependsOnBehavior>()
                  .Select(behavior => behavior.Identity)
                  .ToArray());
            }
        }
    }
}