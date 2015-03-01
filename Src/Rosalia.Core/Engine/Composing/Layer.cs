namespace Rosalia.Core.Engine.Composing
{
    using System.Linq;
    using Rosalia.Core.Tasks;

    public class Layer
    {
        private readonly ExecutableWithIdentity[] _items;

        public Layer(ExecutableWithIdentity[] items)
        {
            _items = items;
        }

        public ExecutableWithIdentity[] Items
        {
            get { return _items; }
        }

        public bool IsEmpty
        {
            get { return _items.Length == 0; }
        }

        public bool Contains(ITask<object> task)
        {
            return _items.Any(item => item.Task == task);
        }

        public bool Contains(Identity id)
        {
            return _items.Any(item => item.Id.Equals(id));
        }
    }
}