using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    internal class Derived2 : BaseType
    {
        public int Other { get; set; }

        protected bool Equals(Derived2 other)
        {
            return base.Equals(other) && Other == other.Other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Derived2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Other;
            }
        }
    }
}