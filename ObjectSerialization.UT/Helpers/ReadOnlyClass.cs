namespace ObjectSerialization.UT.Helpers
{
    public class ReadOnlyClass
    {
        public readonly int ReadOnlyInt;
        public int ReadWriteInt;

        public ReadOnlyClass() { }
        public ReadOnlyClass(int readOnlyInt)
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}