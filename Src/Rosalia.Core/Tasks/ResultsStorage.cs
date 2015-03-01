namespace Rosalia.Core.Tasks
{
    public static class ResultsStorage
    {
        public static readonly IResultsStorage Empty = new Nil();
    }

    internal class Cons : IResultsStorage
    {
        private readonly IResultsStorage _tail;
        private readonly Identity _headIdentity;
        private readonly object _value;

        internal Cons(IResultsStorage tail, Identity headIdentity, object value)
        {
            _tail = tail;
            _headIdentity = headIdentity;
            _value = value;
        }

        public T GetValueOf<T>(Identity identity)
        {
            if (_headIdentity.Equals(identity))
            {
                return (T) _value;
            }

            return _tail.GetValueOf<T>(identity);
        }

        public IResultsStorage CreateDerived<T>(Identity task, T value)
        {
            return new Cons(this, task, value);
        }
    }

    internal class Nil : IResultsStorage
    {
        public T GetValueOf<T>(Identity identity)
        {
            return default(T);
        }

        public IResultsStorage CreateDerived<T>(Identity task, T value)
        {
            return new Cons(this, task, value);
        }
    }
}