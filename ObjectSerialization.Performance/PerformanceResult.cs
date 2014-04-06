namespace ObjectSerialization.Performance
{
    class PerformanceResult
    {
        public Measurement DeserializeTime { get; private set; }

        public string Failure { get; set; }
        public Measurement SerializeTime { get; private set; }
        public string SerializerName { get; private set; }
        public int MeasurementCount { get; private set; }
        public long Size { get; set; }
        public string TestCase { get; private set; }

        public PerformanceResult(string testCase, string serializerName, int measurementCount)
        {
            SerializeTime = new Measurement(measurementCount);
            DeserializeTime = new Measurement(measurementCount);
            TestCase = testCase;
            SerializerName = serializerName;
            MeasurementCount = measurementCount;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Failure))
                return string.Format("{0} ({1}):\n {2}\n\n", TestCase, SerializerName, Failure);

            return string.Format("{0} ({1}):\n Serialized Data Size: {2},\n Serialization Time: {3},\n Deserialization Time: {4}\n\n",
                TestCase, SerializerName, Size, SerializeTime, DeserializeTime);
        }
    }
}