using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal class ClassWithoutParameterlessCtor
    {
        [ProtoMember(1)]
        public int Value { get; set; }

        public ClassWithoutParameterlessCtor(int value)
        {
            Value = value;
        }

        protected bool Equals(ClassWithoutParameterlessCtor other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClassWithoutParameterlessCtor)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}