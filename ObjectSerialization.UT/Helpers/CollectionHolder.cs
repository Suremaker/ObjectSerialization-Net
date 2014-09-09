using System.Collections.Generic;

namespace ObjectSerialization.UT.Helpers
{
    class CollectionHolder
    {
        public IList<int> List { get; set; }
        public ICollection<int> Collection { get; set; }
        public IEnumerable<int> Enumerable { get; set; }
    }
}