using System;

namespace ObjectSerialization.UT.Helpers
{
    public class StructHolder
    {
        public ComplexStruct Complex { get; set; }
        public DateTime Date { get; set; }
        public int? Int { get; set; }
        public int? Int2 { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StructHolder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Int.GetHashCode();
                hashCode = (hashCode * 397) ^ Int2.GetHashCode();
                hashCode = (hashCode * 397) ^ Complex.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(StructHolder other)
        {
            return Date.Equals(other.Date) && Int == other.Int && Int2 == other.Int2 && Complex.Equals(other.Complex);
        }
    }
}