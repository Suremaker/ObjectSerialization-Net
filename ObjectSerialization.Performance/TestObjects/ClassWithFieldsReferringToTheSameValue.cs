using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    internal class ClassWithFieldsReferringToTheSameValue
    {
        [ProtoMember(1)]
        [DataMember]
        public SimpleClass A { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public SimpleClass B { get; set; }

        public ClassWithFieldsReferringToTheSameValue() { }

        public ClassWithFieldsReferringToTheSameValue(SimpleClass simpleClass)
        {
            A = simpleClass;
            B = simpleClass;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClassWithFieldsReferringToTheSameValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((B != null ? B.GetHashCode() : 0) * 397) ^ (A != null ? A.GetHashCode() : 0);
            }
        }

        protected bool Equals(ClassWithFieldsReferringToTheSameValue other)
        {
            if (!Equals(B, other.B) && Equals(A, other.A))
                return false;

            return !ReferenceEquals(other.A, other.B) || (ReferenceEquals(other.A, other.B) && ReferenceEquals(A, B));
        }
    }
}