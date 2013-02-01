using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class ClassMembersSerializerBuilder<T> : SerializerBuilder
    {
        public static Func<BinaryReader, object> DeserializeFn { get; private set; }
        public static Action<BinaryWriter, object> SerializeFn { get; private set; }

        static ClassMembersSerializerBuilder()
        {
            Build();
        }

        private static void Build()
        {
            var ctx = new BuildContext<T>(Expression.Variable(typeof(T), "o"));

            IOrderedEnumerable<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                                   .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null)
                                                                   .OrderBy(p => p.Name);

            IOrderedEnumerable<FieldInfo> fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)
                                                             .Where(f => !f.IsInitOnly).OrderBy(f => f.Name);

            foreach (PropertyInfo property in properties)
                BuildPropertySerializer(property, ctx);

            foreach (FieldInfo field in fields)
                BuildFieldSerializer(field, ctx);

            SerializeFn = ctx.GetSerializeFn();
            DeserializeFn = ctx.GetDeserializeFn();
        }

        private static void BuildFieldSerializer(FieldInfo field, BuildContext<T> ctx)
        {
            ISerializer serializer = Serializers.First(s => s.IsSupported(field.FieldType));
            ctx.AddWriteExpression(serializer.Write(ctx.WriterObject, GetFieldValue(ctx.WriteObject, field), field.FieldType));
            ctx.AddReadExpression(SetFieldValue(ctx.ReadResultObject, field, serializer.Read(ctx.ReaderObject, field.FieldType)));
        }

        private static void BuildPropertySerializer(PropertyInfo property, BuildContext<T> ctx)
        {
            ISerializer serializer = Serializers.First(s => s.IsSupported(property.PropertyType));
            ctx.AddWriteExpression(serializer.Write(ctx.WriterObject, GetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.AddReadExpression(SetPropertyValue(ctx.ReadResultObject, property, serializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static Expression SetFieldValue(Expression instance, FieldInfo field, Expression valueExpression)
        {
            return Expression.Assign(Expression.Field(instance, field), valueExpression);
        }

        private static Expression GetFieldValue(Expression instance, FieldInfo field)
        {
            return Expression.Field(instance, field);
        }

        private static Expression GetPropertyValue(ParameterExpression instance, PropertyInfo property)
        {
            return Expression.Property(instance, property);
        }

        private static Expression SetPropertyValue(Expression instance, PropertyInfo property, Expression valueExpression)
        {
            return Expression.Call(instance, property.GetSetMethod(), valueExpression);
        }
    }
}