namespace ObjectSerialization.Performance
{
    class PerformanceResult
    {
        public Measurement DeserializeTime { get; private set; }

        public string Failure { get; set; }
        public Measurement SerializeTime { get; private set; }
        public string SerializerName { get; private set; }
        public long Size { get; set; }
        public string TestCase { get; private set; }

        public PerformanceResult(string testCase, string serializerName)
        {
            SerializeTime = new Measurement();
            DeserializeTime = new Measurement();
            TestCase = testCase;
            SerializerName = serializerName;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Failure))
                return string.Format("{0}[{1}]: {2}\n\n", SerializerName, TestCase, Failure);

            return string.Format("{0}[{1}]:\n Serialized Data Size: {2},\n Serialization Time: {3},\n Deserialization Time: {4}\n\n",
                SerializerName, TestCase, Size, SerializeTime, DeserializeTime);
        }
    }
}