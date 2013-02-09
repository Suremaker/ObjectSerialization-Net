using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    [ProtoInclude(2, typeof(Derived))]
    [ProtoInclude(3, typeof(Derived2))]
    [DataContract]
    [KnownType(typeof(Derived))]
    [KnownType(typeof(Derived2))]
    internal class BaseType
    {
        [ProtoMember(1)]
        [DataMember]
        public double Value { get; set; }

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

        protected bool Equals(BaseType other)
        {
            return Value.Equals(other.Value);
        }
    }
}