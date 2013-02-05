namespace ObjectSerialization.UT.Helpers
{
    public class DerivedClassWithOverriddenTransientMembers : BaseClassWithTransientMembers
    {
        public override string TransientProperty { get; set; }
        public string Other { get; set; }

        public string BaseTransientProperty { get { return base.TransientProperty; } set { base.TransientProperty = value; } }
    }
}