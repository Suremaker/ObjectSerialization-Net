using System;
using System.IO;

namespace ObjectSerialization.Types
{
	internal class TypeInfo
	{
		public Type Type;
		public string LongTypeId;
		public int? ShortTypeId;
		public Func<BinaryReader, object> Deserializer;
		public Action<BinaryWriter, object> Serializer;
	}
}