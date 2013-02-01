using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    class BasicTypes
    {
        [ProtoMember(1)]
        public bool Bool { get; set; }
        [ProtoMember(2)]
        public byte Byte { get; set; }
        [ProtoMember(3)]
        public char Character { get; set; }
        [ProtoMember(4)]
        public decimal Decimal { get; set; }
        [ProtoMember(5)]
        public double Double { get; set; }
        [ProtoMember(6)]
        public float Float { get; set; }
        [ProtoMember(7)]
        public int Int { get; set; }
        [ProtoMember(8)]
        public long Long { get; set; }
        [ProtoMember(9)]
        public sbyte SByte { get; set; }
        [ProtoMember(10)]
        public short Short { get; set; }
        [ProtoMember(11)]
        public string String { get; set; }
        [ProtoMember(12)]
        public uint UInt { get; set; }
        [ProtoMember(13)]
        public ulong ULong { get; set; }
        [ProtoMember(14)]
        public ushort UShort { get; set; }

        protected bool Equals(BasicTypes other)
        {
            return Bool.Equals(other.Bool) && Byte == other.Byte && Character == other.Character &&
                   Decimal == other.Decimal && Double.Equals(other.Double) && Float.Equals(other.Float) &&
                   Int == other.Int && Long == other.Long && SByte == other.SByte && Short == other.Short &&
                   string.Equals(String, other.String) && UInt == other.UInt && UShort == other.UShort &&
                   ULong == other.ULong;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BasicTypes)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Bool.GetHashCode();
                hashCode = (hashCode * 397) ^ Byte.GetHashCode();
                hashCode = (hashCode * 397) ^ Character.GetHashCode();
                hashCode = (hashCode * 397) ^ Decimal.GetHashCode();
                hashCode = (hashCode * 397) ^ Double.GetHashCode();
                hashCode = (hashCode * 397) ^ Float.GetHashCode();
                hashCode = (hashCode * 397) ^ Int;
                hashCode = (hashCode * 397) ^ Long.GetHashCode();
                hashCode = (hashCode * 397) ^ SByte.GetHashCode();
                hashCode = (hashCode * 397) ^ Short.GetHashCode();
                hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)UInt;
                hashCode = (hashCode * 397) ^ UShort.GetHashCode();
                hashCode = (hashCode * 397) ^ ULong.GetHashCode();
                return hashCode;
            }
        }
    }
}
