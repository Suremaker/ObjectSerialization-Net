namespace ObjectSerialization.UT.Helpers
{
    public class PolyImpl : APoly
    {
        public string Text { get; set; }

        public override int Int { get; set; }

        protected bool Equals(PolyImpl other)
        {
            return string.Equals(Text, other.Text) && Int == other.Int;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PolyImpl)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Text != null ? Text.GetHashCode() : 0) * 397) ^ Int;
            }
        }
    }
}