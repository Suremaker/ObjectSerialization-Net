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

		public override string ToString()
		{
			return string.Format("Short: {0}, Long: {1}, Type: {2}",
				(ShortTypeId != null) ? ShortTypeId.ToString() : "none",
				LongTypeId,
				Type.AssemblyQualifiedName);
		}
	}
}