namespace ObjectSerialization.UT.Helpers
{
    public struct ReadOnlyStruct
    {
        public int ReadWriteInt;
        private readonly int _readOnlyInt;

        public int ReadOnlyProperty { get; private set; }
        public int ReadWriteProperty { get; set; }

        public int ReadOnlyInt
        {
            get { return _readOnlyInt; }
        }

        public ReadOnlyStruct(int readWriteInt, int readOnlyInt)
            : this()
        {
            ReadWriteInt = readWriteInt;
            _readOnlyInt = readOnlyInt;
            ReadWriteProperty = readWriteInt;
            ReadOnlyProperty = readOnlyInt;
        }
    }
}