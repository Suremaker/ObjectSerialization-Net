namespace ObjectSerialization.UT.Helpers
{
    public class ReadOnlyClass
    {
        public int ReadWriteInt;
        public readonly int ReadOnlyInt;

        public ReadOnlyClass() { }
        public ReadOnlyClass(int readOnlyInt)
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}