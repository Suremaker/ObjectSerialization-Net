using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using ObjectSerialization.UT.Helpers;

namespace ObjectSerialization.UT
{
    [TestFixture]
    public class ObjectSerializerTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _serializer = new ObjectSerializer();
        }

        #endregion

        private IObjectSerializer _serializer;

        private void AssertProperties<T>(T expected, T actual)
        {
            Assert.That(actual, Is.Not.Null);
            foreach (PropertyInfo prop in typeof(T).GetProperties())
                Assert.That(prop.GetValue(actual, null), Is.EqualTo(prop.GetValue(expected, null)), prop.Name);
        }

        [Test]
        public void ShouldDisallowReadonlyFieldSerializationInClass()
        {
            var ex = Assert.Throws<SerializationException>(() => _serializer.Serialize(new ReadOnlyClass(66)));
            Assert.That(ex.Message, Is.EqualTo("Unable to serialize readonly field ReadOnlyInt in type ObjectSerialization.UT.Helpers.ReadOnlyClass. Please mark it with NonSerialized attribute or remove readonly modifier."));
        }

        [Test]
        public void ShouldDisallowReadonlyFieldSerializationInStruct()
        {
            var ex = Assert.Throws<SerializationException>(() => _serializer.Serialize(new ReadOnlyStruct(32, 66)));
            Assert.That(ex.Message, Is.EqualTo("Unable to serialize readonly field _readOnlyInt in type ObjectSerialization.UT.Helpers.ReadOnlyStruct. Please mark it with NonSerialized attribute or remove readonly modifier."));
        }

        [Test]
        public void ShouldOmitDerivedPropertyWithNonSerializedBackend()
        {
            var expected = new DerivedClassWithTransientMembers { Text = "text", TransientProperty = "transient", Other = "test" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<DerivedClassWithTransientMembers>(serialized);
            Assert.That(actual.Text, Is.EqualTo(expected.Text));
            Assert.That(actual.Other, Is.EqualTo(expected.Other));
            Assert.That(actual.TransientProperty, Is.Null);
        }

        [Test]
        public void ShouldOmitNonSerializedFieldsInClass()
        {
            var expected = new ClassWithTransientMembers(33) { ReadWriteInt = 55 };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ClassWithTransientMembers>(serialized);
            Assert.That(actual.ReadWriteInt, Is.EqualTo(expected.ReadWriteInt));
            Assert.That(actual.ReadOnlyInt, Is.EqualTo(0));
        }

        [Test]
        public void ShouldOmitNonSerializedFieldsInStruct()
        {
            var expected = new StructWithTransientMembers(33) { ReadWriteInt = 55 };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<StructWithTransientMembers>(serialized);
            Assert.That(actual.ReadWriteInt, Is.EqualTo(expected.ReadWriteInt));
            Assert.That(actual.ReadOnlyInt, Is.EqualTo(0));
        }

        [Test]
        public void ShouldOmitPropertyWithNonSerializedBackend()
        {
            var expected = new BaseClassWithTransientMembers { Text = "text", TransientProperty = "transient" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<BaseClassWithTransientMembers>(serialized);
            Assert.That(actual.Text, Is.EqualTo(expected.Text));
            Assert.That(actual.TransientProperty, Is.Null);
        }

        [Test]
        public void ShouldOmitPropertyWithNonSerializedBackendInBaseClass()
        {
            var expected = new DerivedClassWithOverriddenTransientMembers { Text = "text", TransientProperty = "transient", Other = "test", BaseTransientProperty = "transient2" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<DerivedClassWithOverriddenTransientMembers>(serialized);
            Assert.That(actual.Text, Is.EqualTo(expected.Text));
            Assert.That(actual.Other, Is.EqualTo(expected.Other));
            Assert.That(actual.TransientProperty, Is.EqualTo(expected.TransientProperty));
            Assert.That(actual.BaseTransientProperty, Is.Null);
        }

        [Test]
        public void ShouldSerializeArray()
        {
            var expected = new byte[] { 1, 2, 3, 4, 5 };
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EquivalentTo(expected));
        }

        [Test]
        public void ShouldSerializeArrayMember()
        {
            var expected = new ArrayHolder
                {
                    ByteArray = new byte[] { 1, 2, 3 },
                    SimpleTypeArray = new[] { new SimpleType { TextA = "11", TextB = "12" }, new SimpleType2 { TextA = "21" } },
                    ObjectArray = new object[] { new SimpleType { TextA = "a", TextB = "b" }, 5, "test" }
                };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ArrayHolder>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeBasicTypeMembers()
        {
            var expected = new BasicTypes
            {
                Character = 'a',
                Int = 3,
                Short = 2,
                Byte = 1,
                Long = 6,
                Double = 4.4,
                Float = 3.2f,
                UShort = 2,
                ULong = 3,
                UInt = 4,
                Decimal = 44,
                SByte = 3,
                Bool = false,
                String = "test"
            };

            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<BasicTypes>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeCollection()
        {
            var expected = new List<SimpleType> { new SimpleType { TextA = "a" }, new SimpleType2 { TextB = "b" } };
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EquivalentTo(expected));
        }

        [Test]
        public void ShouldSerializeEmptyClass()
        {
            var expected = new EmptyClass();
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldSerializeEmptyClassMember()
        {
            var expected = new ObjectHolder { Value = new EmptyClass() };
            byte[] serialized = _serializer.Serialize(expected);
            AssertProperties(expected, _serializer.Deserialize<object>(serialized));
        }

        [Test]
        public void ShouldSerializeEmptyStruct()
        {
            var expected = new EmptyStruct();
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldSerializeEmptyStructMember()
        {
            var expected = new ObjectHolder { Value = new EmptyStruct() };
            byte[] serialized = _serializer.Serialize(expected);
            AssertProperties(expected, _serializer.Deserialize<object>(serialized));
        }

        [Test]
        public void ShouldSerializeInt()
        {
            const int expected = 5;
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldSerializeNestedPolymorphicType()
        {
            var expected = new NestedType
            {
                Int = 32,
                Simple = new SimpleType2 { TextA = "test", TextB = "test2" }
            };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<NestedType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeNestedType()
        {
            var expected = new NestedType
            {
                Int = 32,
                Simple = new SimpleType { TextA = "test", TextB = "test2" }
            };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<NestedType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeNestedTypeWhereNestedElementIsNull()
        {
            var expected = new NestedType
            {
                Int = 32,
                Simple = null
            };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<NestedType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeNullArrayMember()
        {
            var expected = new ArrayHolder
                {
                    ByteArray = new byte[] { 1, 2, 3 },
                    ObjectArray = new object[] { new SimpleType { TextA = "a", TextB = "b" }, 5, "test", null }
                };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ArrayHolder>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeNullStringMember()
        {
            var expected = new SimpleType { TextA = "test", TextB = null };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeNullValue()
        {
            byte[] serialized = _serializer.Serialize(null);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.Null);
        }

        [Test]
        public void ShouldSerializeObjectMemberWithAssignedArray()
        {
            var expected = new ObjectHolder { Value = new[] { 1, 2, 3, 4, 5 } };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ObjectHolder>(serialized);
            Assert.That(actual.Value, Is.EqualTo(expected.Value));
        }

        [Test]
        public void ShouldSerializeObjectMemberWithAssignedCollection()
        {
            var dictionary = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
            var expected = new ObjectHolder { Value = dictionary };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ObjectHolder>(serialized);
            Assert.That(actual.Value, Is.EqualTo(expected.Value));
        }

        [Test]
        public void ShouldSerializeObjectMemberWithAssignedValueType()
        {
            var expected = new ObjectHolder { Value = 5 };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ObjectHolder>(serialized);
            Assert.That(actual.Value, Is.EqualTo(expected.Value));
        }

        [Test]
        public void ShouldSerializePolymorphicMemberTypes()
        {
            var expected = new PolyHolder
                {
                    IntPoly = new PolyImpl { Int = 1, Text = "a" },
                    AbsPoly = new PolyImpl { Int = 2, Text = "b" },
                    ObjPoly = new PolyImpl { Int = 3, Text = "c" }
                };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<PolyHolder>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializePolymorphicTypes()
        {
            var expected = new OtherType
            {
                Object = new PolyImpl { Text = "aaa", Int = 3 },
                Interface = new PolyImpl { Text = "bbb", Int = 4 },
                Abstract = new PolyImpl { Text = "ccc", Int = 5 }
            };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<OtherType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializePolymorphicTypesWithNullValues()
        {
            var expected = new OtherType
            {
                Object = null,
                Interface = null,
                Abstract = null
            };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<OtherType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeSealedMemberType()
        {
            var expected = new SealedHolder { Value = new SealedSimpleType { TextA = "a", TextB = "b" } };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SealedHolder>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeString()
        {
            const string expected = "sss";
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldSerializeStringMember()
        {
            var expected = new SimpleType { TextA = "test", TextB = "other" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void ShouldSerializeStruct()
        {
            object expected = new DateTime(2000, 10, 10);
            byte[] serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize<object>(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldSerializeStructMember()
        {
            object expected = new StructHolder { Date = new DateTime(2005, 05, 07), Int = 5, Int2 = null, Complex = new ComplexStruct { Text = "test", Span = new TimeSpan(1, 2, 3) } };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<object>(serialized);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldThrowExceptionIfArrayMemberTypeDoesNotHaveParameterlessConstructor()
        {
            var expected = new object[] { new ClassWithoutEmptyCtor(33) };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<object>(serialized);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldThrowExceptionIfClassDoesNotHaveParameterlessConstructor()
        {
            var expected = new ClassWithoutEmptyCtor(33);
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<object>(serialized);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldSerializeFieldsReferringToCollectionsViaInterfaceTypes()
        {
            AssertCollectionHolder(new CollectionHolder { List = new List<int> { 1, 2, 3 }, Collection = new List<int> { 4, 5, 6 }, Enumerable = new List<int> { 7, 8, 9 } });
            AssertCollectionHolder(new CollectionHolder { List = new[] { 1, 2, 3 }, Collection = new[] { 4, 5, 6 }, Enumerable = new[] { 7, 8, 9 } });
        }

        private void AssertCollectionHolder(CollectionHolder holder)
        {
            var result = _serializer.Deserialize<CollectionHolder>(_serializer.Serialize(holder));
            Assert.That(result.List, Is.EqualTo(holder.List));
            Assert.That(result.Collection, Is.EqualTo(holder.Collection));
            Assert.That(result.Enumerable, Is.EqualTo(holder.Enumerable));
        }
    }
}
