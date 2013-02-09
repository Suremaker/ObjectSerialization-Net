namespace ObjectSerialization.UT.Helpers
{
    public class ClassWithoutEmptyCtor
    {
        public int Value { get; set; }

        public ClassWithoutEmptyCtor(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClassWithoutEmptyCtor)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        protected bool Equals(ClassWithoutEmptyCtor other)
        {
            return Value == other.Value;
        }
    }
}