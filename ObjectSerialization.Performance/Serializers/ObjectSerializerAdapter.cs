using System.IO;

namespace ObjectSerialization.Performance.Serializers
{
    internal class ObjectSerializerAdapter : ISerializerAdapter
    {
        private readonly IObjectSerializer _serializer = new ObjectSerializer();

        #region ISerializerAdapter Members

        public string Name { get { return "ObjectSerializer"; } }
        public T Deserialize<T>(Stream stream, out long operationTime)
        {
            return ExecutionTimer.Measure(() => _serializer.Deserialize<T>(stream), out operationTime);
        }

        public void Serialize<T>(Stream stream, T value, out long operationTime)
        {
            ExecutionTimer.Measure(() => _serializer.Serialize(stream, value), out operationTime);
        }

        #endregion
    }
}