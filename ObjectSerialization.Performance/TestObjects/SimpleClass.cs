using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    class SimpleClass
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public double Double { get; set; }

        protected bool Equals(SimpleClass other)
        {
            return Number == other.Number && string.Equals(Text, other.Text) && Double.Equals(other.Double);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SimpleClass)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Number;
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Double.GetHashCode();
                return hashCode;
            }
        }
    }
}