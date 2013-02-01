using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSerialization
{
    internal class TypeSerializerBuilder
    {
        protected static readonly IEnumerable<ISerializer> Serializers = new ISerializer[]
            {
                new ArrayTypeSerializer(),                
                new PredefinedTypeSerializer(),
                new CollectionTypeSerializer(),
                new ClassTypeSerializer(),
                new ValueTypeSerializer()
            };
    }

    internal class TypeSerializerBuilder<T> : TypeSerializerBuilder
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
                BuildPropertySerializer(property, ctx);

            SerializeFn = ctx.GetSerializeFn();
            DeserializeFn = ctx.GetDeserializeFn();

        }

        private static void BuildPropertySerializer(PropertyInfo property, Context ctx)
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