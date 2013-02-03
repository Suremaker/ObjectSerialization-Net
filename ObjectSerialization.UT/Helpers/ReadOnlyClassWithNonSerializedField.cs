using System;

namespace ObjectSerialization.UT.Helpers
{
    public struct ReadOnlyClassWithNonSerializedField
    {
        public int ReadWriteInt;
        [NonSerialized]
        public readonly int ReadOnlyInt;

        public ReadOnlyClassWithNonSerializedField(int readOnlyInt)
            : this()
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}