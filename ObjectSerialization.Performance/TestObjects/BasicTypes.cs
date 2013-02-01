using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    class BasicTypes
    {
        public bool Bool { get; set; }
        public byte Byte { get; set; }
        public char Character { get; set; }
        public decimal Decimal { get; set; }

        public double Double { get; set; }

        public float Float { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }

        public sbyte SByte { get; set; }
        public short Short { get; set; }

        public string String { get; set; }
        public uint UInt { get; set; }
        public ulong ULong { get; set; }
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
                var hashCode = Bool.GetHashCode();
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
