using System;
using ProtoBuf;

namespace ObjectSerialization.Performance.TestObjects
{
	[Serializable]
    [ProtoContract]
	internal class RegisteredSimpleClass : SimpleClass { }
}