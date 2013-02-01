ObjectSerialization-Net
===========

A fast object graph binary serialization for .NET.

Features:
* support for serialization of classes, sructures and primitives at root or type member level;
* support for serialization of derived types referenced by interface, base class or abstract class type;

Usage:
```c#
IObjectSerializer serializer = new ObjectSerializer();
MyClass myObject = new MyClass { Number = 32, Double = 3.14, Text = "test" };
byte[] serializedData = serializer.Serialize(myObject);
MyClass deserialized = serializer.Deserialize<MyClass>(serializedData);
```
 