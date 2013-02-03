using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class StructMembersSerializerBuilder<T> : SerializerBuilder
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
            var ctx = new BuildContext<T>(Expression.Variable(typeof(T), "o"));

            IOrderedEnumerable<FieldInfo> fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0)
                .OrderBy(p => p.Name);

            foreach (FieldInfo field in fields)
                BuildFieldSerializer(field, ctx);

            _serializeFn = ctx.GetSerializeFn();
            _deserializeFn = ctx.GetDeserializeFn();

        }

        private static void BuildFieldSerializer(FieldInfo field, BuildContext<T> ctx)
        {
            if (field.IsInitOnly)
                throw new SerializationException(string.Format("Unable to serialize readonly field {0} in type {1}. Please mark it with NonSerialized attribute or remove readonly modifier.", field.Name, typeof(T).FullName));

            ISerializer serializer = Serializers.First(s => s.IsSupported(field.FieldType));
            ctx.AddWriteExpression(serializer.Write(ctx.WriterObject, GetField(ctx.WriteObject, field), field.FieldType));
            ctx.AddReadExpression(SetField(ctx.ReadResultObject, field, serializer.Read(ctx.ReaderObject, field.FieldType)));
        }

        private static Expression GetField(Expression instance, FieldInfo field)
        {
            return Expression.Field(instance, field);
        }

        private static Expression SetField(Expression instance, FieldInfo field, Expression valueExpression)
        {
            return Expression.Assign(Expression.Field(instance, field), valueExpression);
        }
    }
}