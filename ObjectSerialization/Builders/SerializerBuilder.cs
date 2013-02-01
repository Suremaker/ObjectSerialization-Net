using System.Collections.Generic;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class SerializerBuilder
    {
        protected static readonly IEnumerable<ISerializer> Serializers = new ISerializer[]
        {
            new ArrayTypeSerializer(),                
            new NullablePredefinedTypeSerializer(),
            new PredefinedTypeSerializer(),
            new CollectionTypeSerializer(),
            new ClassTypeSerializer(),
            new ValueTypeSerializer()
        };
    }
}