namespace ObjectSerialization.UT.Helpers
{
    public class DerivedClassWithOverriddenTransientMembers : BaseClassWithTransientMembers
    {
        public string BaseTransientProperty { get { return base.TransientProperty; } set { base.TransientProperty = value; } }
        public string Other { get; set; }
        public override string TransientProperty { get; set; }
    }
}