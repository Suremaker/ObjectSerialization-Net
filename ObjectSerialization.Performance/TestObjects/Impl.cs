using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [Serializable]
    [ProtoContract]
    internal class Impl : IInterface
    {
        [ProtoMember(1)]
        public string Text { get; set; }

        protected bool Equals(Impl other)
        {
            return string.Equals(Text, other.Text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Impl)obj);
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0);
        }
    }
}