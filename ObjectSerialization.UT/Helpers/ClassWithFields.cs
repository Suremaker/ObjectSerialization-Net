namespace ObjectSerialization.UT.Helpers
{
    public class ClassWithReadOnlyMembers
    {
        public int ReadWriteField;

        public readonly int ReadOnlyField;
        public ClassWithReadOnlyMembers() { }
        public ClassWithReadOnlyMembers(int readOnlyField, int readOnlyProperty)
        {
            ReadOnlyField = readOnlyField;
            ReadOnlyProperty = readOnlyProperty;
        }

        public int ReadWriteProperty { get; set; }
        public int ReadOnlyProperty { get; private set; }
    }
}