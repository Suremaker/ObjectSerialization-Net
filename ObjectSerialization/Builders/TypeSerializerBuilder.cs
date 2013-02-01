using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

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
            var ctx = new BuildContext<T>(Expression.Default(typeof(T)));
            BuildTypeSerializer(typeof(T), ctx);
            SerializeFn = ctx.GetSerializeFn();
            DeserializeFn = ctx.GetDeserializeFn();

        }

        private static void BuildTypeSerializer(Type type, BuildContext<T> ctx)
        {
            var serializer = Serializers.First(s => s.IsSupported(type));

            Expression writeObject = type.IsValueType ? Expression.Convert(ctx.WriteObject, type) : Expression.TypeAs(ctx.WriteObject, type);

            ctx.WriteExpressions.Add(serializer.Write(ctx.WriterObject, writeObject, type));
            ctx.ReadExpressions.Add(Expression.Assign(ctx.ReadResultObject, serializer.Read(ctx.ReaderObject, type)));
        }
    }
}