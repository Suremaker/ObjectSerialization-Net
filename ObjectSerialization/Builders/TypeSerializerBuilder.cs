using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class TypeSerializerBuilder<T> : SerializerBuilder
    {
        public static Func<BinaryReader, object> DeserializeFn { get; private set; }
        public static Action<BinaryWriter, object> SerializeFn { get; private set; }

        static TypeSerializerBuilder()
        {
            Build();
        }

        private static void Build()
        {
            var ctx = new BuildContext<T>();
            BuildTypeSerializer(typeof(T), ctx);
            SerializeFn = ctx.GetSerializeFn();
            DeserializeFn = ctx.GetDeserializeFn();

        }

        private static void BuildTypeSerializer(Type type, BuildContext<T> ctx)
        {
            ISerializer serializer = Serializers.First(s => s.IsSupported(type));

            ctx.AddWriteExpression(serializer.Write(ctx.WriterObject, ctx.WriteObject, type));
            ctx.AddReadExpression(Expression.Assign(ctx.ReadResultObject, serializer.Read(ctx.ReaderObject, type)));
        }
    }
}