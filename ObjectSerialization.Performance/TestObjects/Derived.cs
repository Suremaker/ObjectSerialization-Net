using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    internal class Derived : BaseType
    {
        public char Other { get; set; }

        protected bool Equals(Derived other)
        {
            return base.Equals(other) && Other == other.Other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Derived) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ Other.GetHashCode();
            }
        }
    }
}