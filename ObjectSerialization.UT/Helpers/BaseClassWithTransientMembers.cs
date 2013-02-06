namespace ObjectSerialization.UT.Helpers
{
    public class BaseClassWithTransientMembers
    {
        public string Text { get; set; }

        [NonSerializedBackend]
        public virtual string TransientProperty { get; set; }
    }
}