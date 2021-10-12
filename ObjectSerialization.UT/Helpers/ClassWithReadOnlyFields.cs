using System;

namespace ObjectSerialization.UT.Helpers
{
    public class ClassWithReadOnlyFields
    {
        public readonly int ReadOnlyInt;
        public readonly string ReadOnlyString;

        public ClassWithReadOnlyFields()
        {
            
        }
        public ClassWithReadOnlyFields(int readOnlyInt, string readOnlyString)
            : this()
        {
            ReadOnlyInt = readOnlyInt;
            ReadOnlyString = readOnlyString;
        }
    }
}