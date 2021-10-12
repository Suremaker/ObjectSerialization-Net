using System;
using System.Reflection;
using NUnit.Framework;
using ObjectSerialization.Builders;
using ObjectSerialization.UT.Helpers;

namespace ObjectSerialization.UT
{
    [TestFixture]
    public class InitOnlyFieldSetterTests
    {
        [Test]
        public void ShouldSetValueFieldOnClass()
        {
            var x = new ClassWithReadOnlyFields();
            var field = typeof(ClassWithReadOnlyFields).GetField(nameof(ClassWithReadOnlyFields.ReadOnlyInt), BindingFlags.Public | BindingFlags.Instance);
            var fn = (InitOnlyFieldSetterDelegate<ClassWithReadOnlyFields, int>)InitOnlyFieldSetter.GetMethod(field);
            fn.Invoke(ref x, 55);
            Assert.That(x.ReadOnlyInt, Is.EqualTo(55));
        }

        [Test]
        public void ShouldSetClassFieldOnClass()
        {
            var x = new ClassWithReadOnlyFields();
            var field = typeof(ClassWithReadOnlyFields).GetField(nameof(ClassWithReadOnlyFields.ReadOnlyString), BindingFlags.Public | BindingFlags.Instance);
            var fn = (InitOnlyFieldSetterDelegate<ClassWithReadOnlyFields, string>)InitOnlyFieldSetter.GetMethod(field);
            fn.Invoke(ref x, "abc");
            Assert.That(x.ReadOnlyString, Is.EqualTo("abc"));
        }

        [Test]
        public void ShouldSetFieldOnStruct()
        {
            var x = new StructWithReadOnlyFields();
            var field = typeof(StructWithReadOnlyFields).GetField(nameof(StructWithReadOnlyFields.ReadOnlyInt), BindingFlags.Public | BindingFlags.Instance);
            var fn = (InitOnlyFieldSetterDelegate<StructWithReadOnlyFields, int>)InitOnlyFieldSetter.GetMethod(field);
            fn.Invoke(ref x, 55);
            Assert.That(x.ReadOnlyInt, Is.EqualTo(55));
        }

        [Test]
        public void ShouldSetClassFieldOnStruct()
        {
            var x = new StructWithReadOnlyFields();
            var field = typeof(StructWithReadOnlyFields).GetField(nameof(StructWithReadOnlyFields.ReadOnlyString), BindingFlags.Public | BindingFlags.Instance);
            var fn = (InitOnlyFieldSetterDelegate<StructWithReadOnlyFields, string>)InitOnlyFieldSetter.GetMethod(field);
            fn.Invoke(ref x, "abc");
            Assert.That(x.ReadOnlyString, Is.EqualTo("abc"));
        }
    }
}
