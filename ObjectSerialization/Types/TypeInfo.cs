using System;
using System.IO;

namespace ObjectSerialization.Types
{
	internal class TypeInfo
	{
	    public Func<BinaryReader, object> Deserializer;
	    public string LongTypeId;
	    public Action<BinaryWriter, object> Serializer;
	    public int? ShortTypeId;
	    public Type Type;
	}
}