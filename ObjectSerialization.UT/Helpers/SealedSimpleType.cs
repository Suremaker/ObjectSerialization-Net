namespace ObjectSerialization.UT.Helpers
{
    sealed class SealedSimpleType
    {
        public string TextA { get; set; }
        public string TextB { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SealedSimpleType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TextA != null ? TextA.GetHashCode() : 0) * 397) ^ (TextB != null ? TextB.GetHashCode() : 0);
            }
        }

        protected bool Equals(SealedSimpleType other)
        {
            return string.Equals(TextA, other.TextA) && string.Equals(TextB, other.TextB);
        }
    }
}