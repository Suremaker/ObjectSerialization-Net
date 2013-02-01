using NUnit.Framework;

namespace ObjectSerialization.UT
{
    class SimpleType
    {
        public string TextA { get; set; }
        public string TextB { get; set; }
    }

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
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TextA, Is.EqualTo(expected.TextA));
            Assert.That(actual.TextB, Is.EqualTo(expected.TextB));
        }
    }
}
