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
        private long _value;

        public CustomStruct(int value)
            : this()
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public bool Equals(CustomStruct other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CustomStruct && Equals((CustomStruct)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}