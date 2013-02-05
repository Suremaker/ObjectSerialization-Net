namespace ObjectSerialization.UT.Helpers
{
    public class DerivedClassWithTransientMembers : BaseClassWithTransientMembers
    {
        [NonSerializedBackend]
        public override string TransientProperty { get; set; }
        public string Other { get; set; }
    }
}