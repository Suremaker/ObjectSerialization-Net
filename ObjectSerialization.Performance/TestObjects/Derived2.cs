﻿using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    [DataContract]
    internal class Derived2 : BaseType
    {
        [ProtoMember(1)]
        [DataMember]
        public int Other { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Derived2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Other;
            }
        }

        protected bool Equals(Derived2 other)
        {
            return base.Equals(other) && Other == other.Other;
        }
    }
}