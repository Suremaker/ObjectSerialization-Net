﻿using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSerialization.Builders
{
    internal class MemberSerializerBuilder<T> : SerializerBuilder
    {
        public static Func<BinaryReader, object> DeserializeFn { get; private set; }
        public static Action<BinaryWriter, object> SerializeFn { get; private set; }

        static MemberSerializerBuilder()
        {
            Build();
        }

        private static void Build()
        {
            var ctx = new BuildContext<T>(Expression.New(typeof(T)));

            IOrderedEnumerable<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                                   .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null)
                                                                   .OrderBy(p => p.Name);

            foreach (PropertyInfo property in properties)
                BuildPropertySerializer(property, ctx);

            SerializeFn = ctx.GetSerializeFn();
            DeserializeFn = ctx.GetDeserializeFn();

        }

        private static void BuildPropertySerializer(PropertyInfo property, BuildContext<T> ctx)
        {
            var serializer = Serializers.First(s => s.IsSupported(property.PropertyType));
            ctx.WriteExpressions.Add(serializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, serializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static Expression GetGetPropertyValue(ParameterExpression instance, PropertyInfo property)
        {
            var castedInstance = Expression.TypeAs(instance, property.DeclaringType);
            return Expression.Property(castedInstance, property);
        }

        private static Expression GetSetPropertyValue(Expression instance, PropertyInfo property, Expression valueExpression)
        {
            return Expression.Call(instance, property.GetSetMethod(), valueExpression);
        }
    }
}