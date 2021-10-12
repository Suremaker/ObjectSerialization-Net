using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ObjectSerialization.Builders
{
    internal delegate void InitOnlyFieldSetterDelegate<TThis, TValue>(ref TThis t, TValue v);

    internal class InitOnlyFieldSetter
    {
        public static Delegate GetMethod(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            var dyn = new DynamicMethod("deserialize_set_" + field.Name, typeof(void), new[] { field.DeclaringType.MakeByRefType(), field.FieldType }, field.DeclaringType);
            var gen = dyn.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (!field.DeclaringType.IsValueType)
                gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);
            var delType = typeof(InitOnlyFieldSetterDelegate<,>).MakeGenericType(field.DeclaringType, field.FieldType);
            return dyn.CreateDelegate(delType);
        }
    }
}