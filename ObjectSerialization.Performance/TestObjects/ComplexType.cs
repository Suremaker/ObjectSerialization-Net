using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal class ComplexType
    {
        [ProtoMember(5)]
        public BaseType BaseType { get; set; }

        [ProtoMember(1)]
        public DateTime Date { get; set; }

        [ProtoMember(3)]
        public IInterface InterfaceHolder { get; set; }

        [ProtoMember(2)]
        public int? NullableInt { get; set; }

        [ProtoMember(4)]
        public object ObjectHolder { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ComplexType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ NullableInt.GetHashCode();
                hashCode = (hashCode * 397) ^ (InterfaceHolder != null ? InterfaceHolder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ObjectHolder != null ? ObjectHolder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BaseType != null ? BaseType.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(ComplexType other)
        {
            return Date.Equals(other.Date) && NullableInt == other.NullableInt &&
                   Equals(InterfaceHolder, other.InterfaceHolder) && Equals(ObjectHolder, other.ObjectHolder) &&
                   Equals(BaseType, other.BaseType);
        }
    }
}