using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [ProtoContract]
    [Serializable]
    [DataContract]
    internal class ClassWithReadonlyField
    {
        [ProtoMember(1)]
        [DataMember]
        public readonly int Value;

        public ClassWithReadonlyField() { }
        public ClassWithReadonlyField(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClassWithReadonlyField)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        protected bool Equals(ClassWithReadonlyField other)
        {
            return Value == other.Value;
        }
    }
}