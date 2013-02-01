namespace ObjectSerialization.Performance.TestCases
{
    internal static class Case
    {
        public static TestCase For<T>(string name, T value)
        {
            return new GenericCase<T>(name, value);
        }

        public static TestCase For<T>(T value)
        {
            return For(typeof(T).Name, value);
        }
    }
}
