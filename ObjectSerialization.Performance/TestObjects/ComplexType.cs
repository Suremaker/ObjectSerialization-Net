using System;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    internal class ComplexType
    {
        public DateTime Date { get; set; }

        public int? NullableInt { get; set; }

        public IInterface InterfaceHolder { get; set; }

        public object ObjectHolder { get; set; }

        public BaseType BaseType { get; set; }

        protected bool Equals(ComplexType other)
        {
            return Date.Equals(other.Date) && NullableInt == other.NullableInt &&
                   Equals(InterfaceHolder, other.InterfaceHolder) && Equals(ObjectHolder, other.ObjectHolder) &&
                   Equals(BaseType, other.BaseType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ComplexType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Date.GetHashCode();
                hashCode = (hashCode*397) ^ NullableInt.GetHashCode();
                hashCode = (hashCode*397) ^ (InterfaceHolder != null ? InterfaceHolder.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ObjectHolder != null ? ObjectHolder.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BaseType != null ? BaseType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}