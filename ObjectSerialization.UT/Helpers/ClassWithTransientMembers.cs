using System;

namespace ObjectSerialization.UT.Helpers
{
    public class ClassWithTransientMembers
    {
        [NonSerialized]
        public readonly int ReadOnlyInt;

        public int ReadWriteInt;

        public ClassWithTransientMembers()
        {
        }

        public ClassWithTransientMembers(int readOnlyInt)
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}