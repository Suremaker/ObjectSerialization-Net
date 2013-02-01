using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal class PolymorphicHolder
    {
        [ProtoMember(1)]
        public BaseType A { get; set; }
        [ProtoMember(2)]
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