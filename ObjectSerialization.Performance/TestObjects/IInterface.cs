using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Impl))]
    internal interface IInterface
    {
    }
}