using System;

namespace ObjectSerialization.Performance.TestObjects
{
    class POCO
    {
        public string Text { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((POCO)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TimeSpan.GetHashCode();
                hashCode = (hashCode * 397) ^ Value;
                return hashCode;
            }
        }

        protected bool Equals(POCO other)
        {
            return string.Equals(Text, other.Text) && TimeSpan.Equals(other.TimeSpan) && Value == other.Value;
        }
    }
}
