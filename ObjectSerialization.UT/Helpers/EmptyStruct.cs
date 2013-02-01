namespace ObjectSerialization.UT.Helpers
{
	public struct EmptyStruct
	{
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is EmptyStruct;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}