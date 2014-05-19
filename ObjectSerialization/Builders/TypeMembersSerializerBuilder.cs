﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using CodeBuilder;
using CodeBuilder.Expressions;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class TypeMembersSerializerBuilder<T> : SerializerBuilder
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

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Concat(GetFields(type.BaseType));
        }

        private static void Build()
        {
            try
            {
                var ctx = new BuildContext<T>(Expr.LocalVariable(typeof(T), "o"), "Members");

                IOrderedEnumerable<FieldInfo> fields = GetFields(typeof(T))
                    .Where(ShouldBePersisted)
                    .OrderBy(f => f.Name);

                foreach (FieldInfo field in fields)
                    BuildFieldSerializer(field, ctx);

                _serializeFn = ctx.GetSerializeFn();
                _deserializeFn = ctx.GetDeserializeFn();
            }
            catch (Exception e)
            {
                throw new SerializationException(e.Message, e);
            }
        }

        private static void BuildFieldSerializer(FieldInfo field, BuildContext<T> ctx)
        {
            if (field.IsInitOnly)
                throw new InvalidOperationException(string.Format("Unable to serialize readonly field {0} in type {1}. Please mark it with NonSerialized attribute or remove readonly modifier.", field.Name, typeof(T).FullName));

            ISerializer serializer = Serializers.First(s => s.IsSupported(field.FieldType));
            ctx.AddWriteExpression(serializer.Write(ctx.WriterObject, GetFieldValue(ctx.WriteObject, field), field.FieldType));
            ctx.AddReadExpression(SetFieldValue(Expr.ReadLocal(ctx.ReadResultObject), field, serializer.Read(ctx.ReaderObject, field.FieldType)));
        }

        private static Expression GetFieldValue(Expression instance, FieldInfo field)
        {
            return Expr.ReadField(instance, field);
        }

        private static PropertyInfo GetPropertyForBackingField(FieldInfo field)
        {
            string propertyName = field.Name.Substring(1, field.Name.IndexOf('>') - 1);
            PropertyInfo propertyForBackingField = field.DeclaringType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return propertyForBackingField;
        }

        private static Expression SetFieldValue(Expression instance, FieldInfo field, Expression valueExpression)
        {
            return Expr.WriteField(instance, field, valueExpression);
        }

        private static bool ShouldBePersisted(FieldInfo field)
        {
            if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0)
                return false;

            if (field.Name.EndsWith("k__BackingField") && GetPropertyForBackingField(field).GetCustomAttributes(typeof(NonSerializedBackendAttribute), true).Length > 0)
                return false;

            return true;
        }
    }
}