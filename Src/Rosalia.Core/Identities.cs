namespace Rosalia.Core
{
    using System.Linq;

    public class Identities
    {
        private readonly Identity[] _items;

        public Identities(params Identity[] items)
        {
            _items = items;
        }

        protected bool Equals(Identities other)
        {
            if (other.Items.Length != Items.Length)
            {
                return false;
            }

            for (int index = 0; index < other.Items.Length; index++)
            {
                Identity identity = other.Items[index];
                Identity thisIdentity = Items[index];

                if (!identity.Equals(thisIdentity))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return _items.GetHashCode();
        }

        public static readonly Identities Empty = new Identities();

        public static Identities operator+(Identities that, Identity identity)
        {
            return that + new Identities(identity);
        }

        public static Identities operator+(Identities that, Identities additional)
        {
            var resultItems = new Identity[that.Items.Length + additional.Items.Length];
            for (var i = 0; i < that.Items.Length; i++)
            {
                resultItems[i] = that.Items[i];
            }

            for (var i = that.Items.Length; i < that.Items.Length + additional.Items.Length; i++)
            {
                resultItems[i] = additional.Items[i - that.Items.Length];
            }

            return new Identities(resultItems.Distinct().ToArray());
        }

        public bool IsEmpty => _items.Length == 0;

        public Identity[] Items => _items;

        public Identity? Find(string value) => _items.FirstOrDefault(item => item.Value == value);

        public bool Contains(Identity key) => _items.Contains(key);

        public override bool Equals(object? obj) => obj is Identities other && Equals(other);
    }
}