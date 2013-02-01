using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    internal class PolymorphicHolder
    {
        public BaseType A { get; set; }
        public BaseType B { get; set; }

        protected bool Equals(PolymorphicHolder other)
        {
            return Equals(A, other.A) && Equals(B, other.B);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PolymorphicHolder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((A != null ? A.GetHashCode() : 0) * 397) ^ (B != null ? B.GetHashCode() : 0);
            }
        }
    }
}