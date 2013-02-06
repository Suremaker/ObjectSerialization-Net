namespace ObjectSerialization.UT.Helpers
{
    public struct ReadOnlyStruct
    {
        private readonly int _readOnlyInt;
        public int ReadWriteInt;

        public int ReadOnlyInt
        {
            get { return _readOnlyInt; }
        }

        public int ReadOnlyProperty { get; private set; }
        public int ReadWriteProperty { get; set; }

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