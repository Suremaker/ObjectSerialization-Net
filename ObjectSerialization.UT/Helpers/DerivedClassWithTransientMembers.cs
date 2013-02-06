namespace ObjectSerialization.UT.Helpers
{
    public class DerivedClassWithTransientMembers : BaseClassWithTransientMembers
    {
        public string Other { get; set; }

        [NonSerializedBackend]
        public override string TransientProperty { get; set; }
    }
}