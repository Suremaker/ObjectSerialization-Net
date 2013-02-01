namespace ObjectSerialization.Performance
{
    class PerformanceResult
    {
        public string TestCase { get; private set; }
        public string SerializerName { get; private set; }
        public Measurement SerializeTime { get; private set; }
        public Measurement DeserializeTime { get; private set; }
        public Measurement Size { get; private set; }

        public string Failure { get; set; }

        public PerformanceResult(string testCase, string serializerName)
        {
            SerializeTime = new Measurement();
            DeserializeTime = new Measurement();
            Size = new Measurement();
            TestCase = testCase;
            SerializerName = serializerName;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Failure))
                return string.Format("{0}[{1}]: {2}", SerializerName, TestCase, Failure);

            return string.Format("{0}[{1}]:\n Serialization Time: {2},\n Deserialization Time: {3},\n Serialized Data Size: {4}\n\n",
                SerializerName, TestCase, SerializeTime, DeserializeTime, Size);
        }
    }
}