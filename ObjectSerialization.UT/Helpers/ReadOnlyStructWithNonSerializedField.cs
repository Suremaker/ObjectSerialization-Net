using System;

namespace ObjectSerialization.UT.Helpers
{
    public struct ReadOnlyStructWithNonSerializedField
    {
        public int ReadWriteInt;
        [NonSerialized]
        public readonly int ReadOnlyInt;

        public ReadOnlyStructWithNonSerializedField(int readOnlyInt)
            : this()
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}