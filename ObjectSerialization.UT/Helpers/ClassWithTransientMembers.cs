using System;

namespace ObjectSerialization.UT.Helpers
{
    public class ClassWithTransientMembers
    {
        public int ReadWriteInt;
        [NonSerialized]
        public readonly int ReadOnlyInt;

        public ClassWithTransientMembers()
        {
        }

        public ClassWithTransientMembers(int readOnlyInt)
        {
            ReadOnlyInt = readOnlyInt;
        }
    }
}