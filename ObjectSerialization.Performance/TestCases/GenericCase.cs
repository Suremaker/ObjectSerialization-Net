using ObjectSerialization.Performance.Serializers;

namespace ObjectSerialization.Performance.TestCases
{
    internal class GenericCase<T> : TestCase
    {
        private readonly string _name;
        private readonly T _value;

        public GenericCase(string name, T value)
        {
            _name = name;
            _value = value;
        }

        public override string Name
        {
            get { return _name; }
        }

        protected override object GetValue()
        {
            return _value;
        }

        protected override byte[] Serialize(ISerializer serializer)
        {
            return serializer.Serialize(_value);
        }

        protected override object Deserialize(ISerializer serializer, byte[] data)
        {
            return serializer.Deserialize<T>(data);
        }
    }
}