using System;

namespace ObjectSerialization.UT.Helpers
{
    public struct StructWithTransientMembers
    {
        public int ReadWriteInt;
        [NonSerialized]
        public readonly int ReadOnlyInt;

        public StructWithTransientMembers(int readOnlyInt)
            : this()
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}