namespace Rosalia.Core.Api.Behaviors
{
    public class DependsOnBehavior : ITaskBehavior
    {
        private readonly Identity _identity;

        public DependsOnBehavior(Identity identity)
        {
            _identity = identity;
        }

        public Identity Identity
        {
            get { return _identity; }
        }
    }
}