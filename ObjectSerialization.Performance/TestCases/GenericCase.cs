using ObjectSerialization.Performance.Serializers;

namespace ObjectSerialization.Performance.TestCases
{
    internal class GenericCase<T> : TestCase
    {
        private readonly string _name;
        private readonly T _value;

        public override string Name
        {
            get { return _name; }
        }

        public GenericCase(string name, T value)
        {
            _name = name;
            _value = value;
        }

        protected override object Deserialize(ISerializerAdapter serializer, byte[] data)
        {
            return serializer.Deserialize<T>(data);
        }

        protected override object GetValue()
        {
            return _value;
        }

        protected override byte[] Serialize(ISerializerAdapter serializer)
        {
            return serializer.Serialize(_value);
        }
    }
}