using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal class Derived : BaseType
    {
        [ProtoMember(1)]
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