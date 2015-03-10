namespace Rosalia.Core
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public class Identity
    {
        private readonly string _value;

        public Identity(string value)
        {
            _value = value;
        }

        public Identity()
        {
            _value = Guid.NewGuid().ToString();
        }

        public string Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Identity) obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        protected bool Equals(Identity other)
        {
            return _value.Equals(other._value);
        }

        public static implicit operator Identity (string value)
        {
            return new Identity(value);
        }
    }
}