using System;

namespace ObjectSerialization
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NonSerializedBackendAttribute : Attribute
    {
    }
}