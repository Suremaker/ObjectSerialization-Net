using System;
using System.Collections.Generic;
using System.Linq;
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
        public PolymorphicHolder PolymorphicHolder { get; set; }

        [ProtoMember(6)]
        public double[] Array { get; set; }

        [ProtoMember(7)]
        public List<Guid> GuidCollection { get; set; }

        protected bool Equals(ComplexType other)
        {
            return Equals(BaseType, other.BaseType) && Date.Equals(other.Date) &&
                   Equals(InterfaceHolder, other.InterfaceHolder) && NullableInt == other.NullableInt &&
                   Equals(PolymorphicHolder, other.PolymorphicHolder) && Array.SequenceEqual(other.Array) &&
                   GuidCollection.SequenceEqual(other.GuidCollection);
        }

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
                var hashCode = (BaseType != null ? BaseType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (InterfaceHolder != null ? InterfaceHolder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ NullableInt.GetHashCode();
                hashCode = (hashCode * 397) ^ (PolymorphicHolder != null ? PolymorphicHolder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Array != null ? Array.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GuidCollection != null ? GuidCollection.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}