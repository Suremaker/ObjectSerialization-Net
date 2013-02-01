﻿namespace ObjectSerialization.Performance.Serializers
{
    internal class DCSerializer : ISerializer
    {
        private readonly IObjectSerializer _serializer = new ObjectSerializer();

        public byte[] Serialize<T>(T value)
        {
            return _serializer.Serialize(value);
        }

        public T Deserialize<T>(byte[] data)
        {
            return _serializer.Deserialize<T>(data);
        }

        public string Name { get { return "ObjectSerializer"; } }
    }
}