namespace ObjectSerialization.Performance.Serializers
{
    internal class ObjectSerializerAdapter : ISerializerAdapter
    {
        private readonly IObjectSerializer _serializer = new ObjectSerializer();

        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value)
        {
            return _serializer.Serialize(value);
        }

        public T Deserialize<T>(byte[] data)
        {
            return _serializer.Deserialize<T>(data);
        }

        public string Name { get { return "ObjectSerializer"; } }

        #endregion
    }
}