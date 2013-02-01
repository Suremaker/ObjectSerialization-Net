using System;
using NUnit.Framework;
using System.Collections.Generic;
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

        private ObjectSerializer _serializer;

        [Test]
        public void StringMemberSerializationTest()
        {
            var expected = new SimpleType { TextA = "test", TextB = "other" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void NullStringMemberSerializationTest()
        {
            var expected = new SimpleType { TextA = "test", TextB = null };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void NullSerializationTest()
        {
            var serialized = _serializer.Serialize(null);
            Assert.That(_serializer.Deserialize(serialized), Is.Null);
        }

        [Test, Ignore("Not implemented yet")]
        public void StructSerializationTest()
        {
            var expected = new DateTime(2000, 10, 10);
            var serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void StringSerializationTest()
        {
            const string expected = "sss";
            var serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void IntSerializationTest()
        {
            const int expected = 5;
            var serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize(serialized), Is.EqualTo(expected));
        }

        [Test]
        public void ValueTypeAssignedToObjectMemberSerializationTest()
        {
            var expected = new ObjectHolder { Value = 5 };
            var serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ObjectHolder>(serialized);
            Assert.That(actual.Value, Is.EqualTo(expected.Value));
        }

        [Test]
        public void ArrayAssignedToObjectMemberSerializationTest()
        {
            var expected = new ObjectHolder { Value = new[] { 1, 2, 3, 4, 5 } };
            var serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ObjectHolder>(serialized);
            Assert.That(actual.Value, Is.EqualTo(expected.Value));
        }

        [Test]
        public void ArraySerializationTest()
        {
            var expected = new byte[] { 1, 2, 3, 4, 5 };
            var serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize(serialized), Is.EquivalentTo(expected));
        }

        [Test, Ignore("Not implemented yet")]
        public void CollectionSerializationTest()
        {
            var expected = new List<SimpleType> { new SimpleType { TextA = "a" }, new SimpleType2 { TextB = "b" } };
            var serialized = _serializer.Serialize(expected);
            Assert.That(_serializer.Deserialize(serialized), Is.EquivalentTo(expected));
        }

        [Test]
        public void ArrayMemberSerializationTest()
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
        public void NullArrayMemberSerializationTest()
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
        public void BasicTypeMemberSerializationTest()
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
        public void NestedTypeSerializationTest()
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
        public void NestedPolymorphicTypeSerializationTest()
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
        public void NestedTypeSerializationTestWhereNestedElementIsNull()
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
        public void PolymorphicTypesSerializationTest()
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
        public void PolymorphicTypesSerializationTestWithNullValues()
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

        private void AssertProperties<T>(T expected, T actual)
        {
            Assert.That(actual, Is.Not.Null);
            foreach (var prop in typeof(T).GetProperties())
                Assert.That(prop.GetValue(actual, null), Is.EqualTo(prop.GetValue(expected, null)), prop.Name);
        }
    }
}
