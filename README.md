ObjectSerialization-Net
===========

A fast object graph binary serialization for .NET.

Features:
* support for serialization of classes, structures and primitives at root or type member level;
* support for serialization of derived types referenced by interface, base class or abstract class type;

Usage:
```c#
IObjectSerializer serializer = new ObjectSerializer();

MyClass myObject = new MyClass { Number = 32, Double = 3.14, Text = "test" };

byte[] serializedData = serializer.Serialize(myObject);

MyClass deserialized = serializer.Deserialize<MyClass>(serializedData);
```

Performance and serialized data size:

The ObjectSerializer performs serialization/deserialization faster than standard BinaryFormatter and produces smaller data.

Solution contains [ObjectSerialization.Performance](tree/master/ObjectSerialization.Performance) project that allows to compare ObjectSerializer performance with other serializers like (BinaryFormatter or Protobuf).

The last performance comparison can be found here: [Performance test results](tree/master/PerformanceResults/results_2013-02-01_17.55.51.html)


 