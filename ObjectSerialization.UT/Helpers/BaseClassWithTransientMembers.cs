namespace ObjectSerialization.UT.Helpers
{
    public class BaseClassWithTransientMembers
    {
        [NonSerializedBackend]
        public virtual string TransientProperty { get; set; }
        public string Text { get; set; }
    }
}