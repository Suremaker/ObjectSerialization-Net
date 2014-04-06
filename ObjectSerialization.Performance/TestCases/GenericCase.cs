using System.IO;
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

        protected override object Deserialize(ISerializerAdapter serializer, Stream stream, out long operationTime)
        {
            try
            {
                return serializer.Deserialize<T>(stream, out operationTime);
            }
            finally { stream.Seek(0, SeekOrigin.Begin); }
        }

        protected override object GetValue()
        {
            return _value;
        }

        protected override void Serialize(ISerializerAdapter serializer, Stream stream, out long operationTime)
        {
            try
            {
                serializer.Serialize(stream, _value, out operationTime);
            }
            finally { stream.Seek(0, SeekOrigin.Begin); }
        }
    }
}