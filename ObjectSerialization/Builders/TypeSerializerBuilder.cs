using System;
using System.IO;
using System.Linq;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class TypeSerializerBuilder<T> : SerializerBuilder
    {
        private static Func<BinaryReader, T> _deserializeFn;
        private static Action<BinaryWriter, T> _serializeFn;

        public static Func<BinaryReader, T> DeserializeFn
        {
            get
            {
                if (_deserializeFn == null)
                    Build();
                return _deserializeFn;
            }
        }

        public static Action<BinaryWriter, T> SerializeFn
        {
            get
            {
                if (_serializeFn == null)
                    Build();
                return _serializeFn;
            }
        }

        public static void SerializeWithCast(BinaryWriter writer, object value)
        {
            SerializeFn(writer, (T)value);
        }

        public static object DeserializeWithCast(BinaryReader reader)
        {
            return DeserializeFn(reader);
        }

        private static void Build()
        {
            var ctx = new BuildContext<T>("");
            BuildTypeSerializer(typeof(T), ctx);
            _serializeFn = ctx.GetSerializeFn();
            _deserializeFn = ctx.GetDeserializeFn();
        }

        private static void BuildTypeSerializer(Type type, BuildContext<T> ctx)
        {
            ISerializer serializer = Serializers.First(s => s.IsSupported(type));

            ctx.AddWriteExpression(serializer.Write(ctx.WriterObject, ctx.WriteObject, type));
            ctx.AddReadExpression(ctx.ReturnValue(serializer.Read(ctx.ReaderObject, type)));
        }
    }
}