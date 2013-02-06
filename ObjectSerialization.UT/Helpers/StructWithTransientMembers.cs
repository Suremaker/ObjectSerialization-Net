using System;

namespace ObjectSerialization.UT.Helpers
{
    public struct StructWithTransientMembers
    {
        [NonSerialized]
        public readonly int ReadOnlyInt;

        public int ReadWriteInt;

        public StructWithTransientMembers(int readOnlyInt)
            : this()
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}