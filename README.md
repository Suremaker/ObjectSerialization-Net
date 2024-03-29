ObjectSerialization-Net
===========

A fast object graph binary serialization for .NET.

## Features:
* support for serialization of classes, structures and primitives at root or type member level;
* support for serialization of derived types referenced by interface, base class or abstract class type;
* support for serialization of POCO objects;
* support for standard `NonSerializedAttribute` and dedicated `NonSerializedBackendAttribute` for automatic properties;
* support for serialization of classes without parameter-less constructor;
* support for serialization of arrays and collections implementing `ICollection<T>`;

## Download
It is possible to download package using [NuGet](http://nuget.org): `PM> Install-Package ObjectSerialization-Net`

or compile from sources available on Git: `git clone git://github.com/Suremaker/ObjectSerialization-Net.git`

## Usage:

```c#
IObjectSerializer serializer = new ObjectSerializer();

MyClass myObject = new MyClass { Number = 32, Double = 3.14, Text = "test" };

byte[] serializedData = serializer.Serialize(myObject);

MyClass deserialized = serializer.Deserialize<MyClass>(serializedData);
```

## Limitations:
* if multiple fields refers to the same object instance, after deserialization each would be referring to own copy of it;
* serialization of object with circular references is not supported (because of above);
* if class does not contain parameter-less constructor, it would be instantiated without any constructor call;
* data serialized on .NetFramework may not be properly or accurately deserialized on .NetCore / .Net5 runtime (and vice versa)

## Performance and serialized data size:

The ObjectSerializer performs serialization/deserialization faster than standard BinaryFormatter and produces smaller data.

Solution contains [ObjectSerialization.Performance](https://github.com/Suremaker/ObjectSerialization-Net/tree/master/ObjectSerialization.Performance) project that allows to compare ObjectSerializer performance with other serializers like (BinaryFormatter, DataContractSerializer, Protobuf or Newton BSON).

The last performance comparison can be found here: [Performance test results](http://htmlpreview.github.io/?https://github.com/Suremaker/ObjectSerialization-Net/blob/master/PerformanceResults/results.html)
