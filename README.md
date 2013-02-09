ObjectSerialization-Net
===========

A fast object graph binary serialization for .NET.

## Features:
* support for serialization of classes, structures and primitives at root or type member level;
* support for serialization of derived types referenced by interface, base class or abstract class type;
* support for serialization of POCO objects;
* support for standard `NonSerializedAttribute` and dedicated `NonSerializedBackendAttribute` for automatic properties;
* support for serialization of classes without parameter-less constructor;

## Limitations:
* serialization of `readonly` fields is not supported (such field has to be marked with `NonSerializedAttribute` or `readonly` modifier has to be removed)

## Usage:

```c#
IObjectSerializer serializer = new ObjectSerializer();

MyClass myObject = new MyClass { Number = 32, Double = 3.14, Text = "test" };

byte[] serializedData = serializer.Serialize(myObject);

MyClass deserialized = serializer.Deserialize<MyClass>(serializedData);
```

## Performance and serialized data size:

The ObjectSerializer performs serialization/deserialization faster than standard BinaryFormatter and produces smaller data.

Solution contains [ObjectSerialization.Performance](https://github.com/Suremaker/ObjectSerialization-Net/tree/master/ObjectSerialization.Performance) project that allows to compare ObjectSerializer performance with other serializers like (BinaryFormatter, DataContractSerializer, Protobuf or Newton BSON).

The last performance comparison can be found here: [Performance test results](https://github.com/Suremaker/ObjectSerialization-Net/blob/master/PerformanceResults/)
