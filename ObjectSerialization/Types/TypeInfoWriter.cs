using System;
using System.IO;

namespace ObjectSerialization.Types
{
	internal static class TypeInfoWriter
	{
	    public static TypeInfo ReadInfo(BinaryReader reader)
		{
			return reader.ReadBoolean() 
				? TypeInfoRepository.GetTypeInfo(reader.ReadInt32()) 
				: TypeInfoRepository.GetTypeInfo(reader.ReadString());
		}

	    public static TypeInfo WriteInfo(BinaryWriter writer, Type type)
	    {
	        TypeInfo info = TypeInfoRepository.GetTypeInfo(type);
	        if (info.ShortTypeId != null)
	        {
	            writer.Write(true);
	            writer.Write(info.ShortTypeId.Value);
	        }
	        else
	        {
	            writer.Write(false);
	            writer.Write(info.LongTypeId);
	        }
	        return info;
	    }
	}
}
