using System;
using System.Globalization;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal struct CustomStruct
    {
        [ProtoMember(1)]
        public long Value { get; private set; }

        public CustomStruct(int value)
            : this()
        {
            Value = value;
        }

        public bool Equals(CustomStruct other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CustomStruct && Equals((CustomStruct)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}