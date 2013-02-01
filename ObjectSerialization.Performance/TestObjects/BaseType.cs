using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    internal class BaseType
    {
        public double Value { get; set; }

        protected bool Equals(BaseType other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BaseType) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}