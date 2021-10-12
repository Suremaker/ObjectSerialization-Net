namespace ObjectSerialization.UT.Helpers
{
    public struct StructWithReadOnlyFields
    {
        public readonly int ReadOnlyInt;
        public readonly string ReadOnlyString;

        public StructWithReadOnlyFields(int readOnlyInt, string readOnlyString)
            : this()
        {
            ReadOnlyInt = readOnlyInt;
            ReadOnlyString = readOnlyString;
        }
    }
}