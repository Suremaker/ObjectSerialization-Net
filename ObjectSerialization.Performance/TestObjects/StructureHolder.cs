using System;
using System.Collections.Generic;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal class StructureHolder
    {
        [ProtoMember(4)]
        public CustomStruct Custom { get; set; }

        [ProtoMember(1)]
        public DateTime Date { get; set; }

        [ProtoMember(5)]
        public Guid Guid { get; set; }

        [ProtoMember(3)]
        public KeyValuePair<DateTime, decimal> Pair { get; set; }

        [ProtoMember(2)]
        public TimeSpan Span { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StructureHolder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Span.GetHashCode();
                hashCode = (hashCode * 397) ^ Pair.GetHashCode();
                hashCode = (hashCode * 397) ^ Custom.GetHashCode();
                hashCode = (hashCode * 397) ^ Guid.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(StructureHolder other)
        {
            return Date.Equals(other.Date) && Span.Equals(other.Span) && Pair.Equals(other.Pair) &&
                   Custom.Equals(other.Custom) && Guid.Equals(other.Guid);
        }
    }
}