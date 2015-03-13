namespace Rosalia.Core.Tasks
{
    public class IdentityWithResult
    {
        private readonly Identity _identity;
        private readonly object _value;

        public IdentityWithResult(Identity identity, object value)
        {
            _identity = identity;
            _value = value;
        }

        public Identity Identity
        {
            get { return _identity; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}