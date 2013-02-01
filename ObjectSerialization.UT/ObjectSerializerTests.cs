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
        public void StringSerializationTest()
        {
            var expected = new SimpleType { TextA = "test", TextB = "other" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test, Ignore("Not implemented")]
        public void NullStringSerializationTest()
        {
            var expected = new SimpleType { TextA = "test", TextB = null };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void SerializedObjectCannotBeNull()
        {
            var ex = Assert.Throws<ArgumentException>(() => _serializer.Serialize(null));
            Assert.That(ex.Message, Is.StringContaining("Serialized type has to be instance of simple POCO type."));
        }

        [Test]
        public void SerializedObjectCannotBeStruct()
        {
            var ex = Assert.Throws<ArgumentException>(() => _serializer.Serialize(new DateTime(2000, 10, 10)));
            Assert.That(ex.Message, Is.StringContaining("Serialized type has to be instance of simple POCO type."));
        }

        [Test]
        public void SerializedObjectCannotBeString()
        {
            var ex = Assert.Throws<ArgumentException>(() => _serializer.Serialize("sss"));
            Assert.That(ex.Message, Is.StringContaining("Serialized type has to be instance of simple POCO type."));
        }

        [Test]
        public void SerializedObjectCannotBeArray()
        {
            var ex = Assert.Throws<ArgumentException>(() => _serializer.Serialize(new byte[5]));
            Assert.That(ex.Message, Is.StringContaining("Serialized type has to be instance of simple POCO type."));
        }

        [Test]
        public void SerializedObjectCannotBeCollection()
        {
            var ex = Assert.Throws<ArgumentException>(() => _serializer.Serialize(new List<SimpleType>()));
            Assert.That(ex.Message, Is.StringContaining("Serialized type has to be instance of simple POCO type."));
        }

        [Test, Ignore("Not implemented yet")]
        public void ArraySerialization()
        {
            var expected = new ArrayHolder
            {
                ByteArray = new byte[] { 1, 2, 3 },
                SimpleTypeArray = new[] { new SimpleType { TextA = "11" }, new SimpleType { TextB = "22" } },
                ObjectArray = new object[] { new SimpleType { TextB = "b" }, 5, "test" }
            };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<ArrayHolder>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void BasicTypeSerializationTest()
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
        public void PolymorphicTypesSerialization()
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
        public void PolymorphicTypesSerializationWithNullValues()
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
