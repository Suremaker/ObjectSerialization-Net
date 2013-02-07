namespace ObjectSerialization.UT.Helpers
{
    public class ClassWithoutEmptyCtor 
    {
        public int Value { get; set; }

        public ClassWithoutEmptyCtor(int value)
        {
            Value = value;
        }
    }
}