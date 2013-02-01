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
        public void SimpleStringPropertiesTest()
        {
            var expected = new SimpleType { TextA = "test", TextB = "other" };
            byte[] serialized = _serializer.Serialize(expected);
            var actual = _serializer.Deserialize<SimpleType>(serialized);
            AssertProperties(expected, actual);
        }

        [Test]
        public void BasicTypeSerializationTests()
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

        private void AssertProperties<T>(T expected, T actual)
        {
            Assert.That(actual, Is.Not.Null);
            foreach (var prop in typeof(T).GetProperties())
                Assert.That(prop.GetValue(actual, null), Is.EqualTo(prop.GetValue(expected, null)), prop.Name);

        }
    }
}
