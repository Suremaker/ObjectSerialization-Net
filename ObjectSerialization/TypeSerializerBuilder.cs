using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSerialization
{
    internal class TypeSerializerBuilder<T> : BaseTypeSerializer
    {
        #region Type: Context

        class Context
        {
            public readonly List<Expression> ReadExpressions = new List<Expression>();

            public readonly ParameterExpression ReadResultObject = Expression.Variable(typeof(T), "o");
            public readonly ParameterExpression ReaderObject = Expression.Parameter(typeof(BinaryReader), "r");
            public readonly List<Expression> WriteExpressions = new List<Expression>();
            public readonly ParameterExpression WriteObject = Expression.Parameter(typeof(object), "o");
            public readonly ParameterExpression WriterObject = Expression.Parameter(typeof(BinaryWriter), "w");


            public Context()
            {
                ReadExpressions.Add(Expression.Assign(ReadResultObject, Expression.New(typeof(T))));
            }

            public Func<BinaryReader, object> GetDeserializeFn()
            {
                LabelTarget label = Expression.Label(typeof(object));
                List<Expression> expressions = ReadExpressions.ToList();
                expressions.Add(Expression.Return(label, ReadResultObject, typeof(object)));
                expressions.Add(Expression.Label(label, Expression.Constant(null, typeof(T))));
                BlockExpression body = Expression.Block(new[] { ReadResultObject }, expressions);
                return Expression.Lambda<Func<BinaryReader, object>>(body, ReaderObject).Compile();
            }

            public Action<BinaryWriter, object> GetSerializeFn()
            {
                return Expression.Lambda<Action<BinaryWriter, object>>(Expression.Block(WriteExpressions), WriterObject, WriteObject).Compile();
            }
        }

        #endregion

        public static Func<BinaryReader, object> DeserializeFn { get; private set; }
        public static Action<BinaryWriter, object> SerializeFn { get; private set; }

        static TypeSerializerBuilder()
        {
            Build();
        }

        private static void Build()
        {
            var ctx = new Context();
            IOrderedEnumerable<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                 .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null)
                                 .OrderBy(p => p.Name);

            foreach (PropertyInfo property in properties)
            {
                Type propType = property.PropertyType;
                if (PredefinedTypeSerializer.IsSupported(propType))
                    BuildPredefined(property, ctx);
                else if (ArrayTypeSerializer.IsSupported(propType))
                    BuildArray(property, ctx);
                else if (CollectionTypeSerializer.IsSupported(propType))
                    BuildCollection(property, ctx);
                else if (PolymorphicClassTypeSerializer.IsSupported(propType))
                    BuildPolymorphic(property, ctx);
                else if (ValueTypeSerializer.IsSupported(propType))
                    BuildValueType(property, ctx);
                else
                    BuildClass(property, ctx);
            }

            SerializeFn = ctx.GetSerializeFn();
            DeserializeFn = ctx.GetDeserializeFn();

        }

        private static void BuildArray(PropertyInfo property, Context ctx)
        {
            ctx.WriteExpressions.Add(ArrayTypeSerializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, ArrayTypeSerializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static void BuildClass(PropertyInfo property, Context ctx)
        {
            ctx.WriteExpressions.Add(ClassTypeSerializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, ClassTypeSerializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static void BuildPolymorphic(PropertyInfo property, Context ctx)
        {
            ctx.WriteExpressions.Add(PolymorphicClassTypeSerializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, PolymorphicClassTypeSerializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static void BuildCollection(PropertyInfo property, Context ctx)
        {
            ctx.WriteExpressions.Add(CollectionTypeSerializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, CollectionTypeSerializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static void BuildPredefined(PropertyInfo property, Context ctx)
        {
            ctx.WriteExpressions.Add(PredefinedTypeSerializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property)));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, PredefinedTypeSerializer.Read(ctx.ReaderObject, property.PropertyType)));
        }

        private static void BuildValueType(PropertyInfo property, Context ctx)
        {
            ctx.WriteExpressions.Add(ValueTypeSerializer.Write(ctx.WriterObject, GetGetPropertyValue(ctx.WriteObject, property), property.PropertyType));
            ctx.ReadExpressions.Add(GetSetPropertyValue(ctx.ReadResultObject, property, ValueTypeSerializer.Read(ctx.ReaderObject, property.PropertyType)));
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