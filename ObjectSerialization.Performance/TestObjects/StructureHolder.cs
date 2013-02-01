using System;
using System.Collections.Generic;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    internal class StructureHolder
    {
        public DateTime Date { get; set; }

        public TimeSpan Span { get; set; }

        public KeyValuePair<DateTime, decimal> Pair { get; set; }
        public CustomStruct Custom { get; set; }
        public Guid Guid { get; set; }

        protected bool Equals(StructureHolder other)
        {
            return Date.Equals(other.Date) && Span.Equals(other.Span) && Pair.Equals(other.Pair) &&
                   Custom.Equals(other.Custom) && Guid.Equals(other.Guid);
        }

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
                var hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Span.GetHashCode();
                hashCode = (hashCode * 397) ^ Pair.GetHashCode();
                hashCode = (hashCode * 397) ^ Custom.GetHashCode();
                hashCode = (hashCode * 397) ^ Guid.GetHashCode();
                return hashCode;
            }
        }
    }
}