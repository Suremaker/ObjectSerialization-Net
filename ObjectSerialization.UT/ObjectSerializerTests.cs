using NUnit.Framework;

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

        [Test,Ignore("Not implemented")]
        public void NullStringSerializationTest()
        {
            var expected = new SimpleType { TextA = "test", TextB = null };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
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

        private void AssertProperties<T>(T expected, T actual)
        {
            Assert.That(actual, Is.Not.Null);
            foreach (var prop in typeof(T).GetProperties())
                Assert.That(prop.GetValue(actual, null), Is.EqualTo(prop.GetValue(expected, null)), prop.Name);

        }
    }
}
