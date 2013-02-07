using System;
using System.IO;
using System.Linq;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class TypeSerializerBuilder<T> : SerializerBuilder
    {
        private static Func<BinaryReader, object> _deserializeFn;
        private static Action<BinaryWriter, object> _serializeFn;

        public static Func<BinaryReader, object> DeserializeFn
        {
            get
            {
                if (_deserializeFn == null)
                    Build();
                return _deserializeFn;
            }
        }

        public static Action<BinaryWriter, object> SerializeFn
        {
            get
            {
                if (_serializeFn == null)
                    Build();
                return _serializeFn;
            }
        }

        private static void Build()
        {
            var ctx = new BuildContext<T>();
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