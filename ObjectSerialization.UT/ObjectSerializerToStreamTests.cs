using System;
using System.IO;
using NUnit.Framework;
using ObjectSerialization.UT.Helpers;

namespace ObjectSerialization.UT
{
	[TestFixture]
	public class ObjectSerializerToStreamTests
	{
		private IObjectSerializer _subject;

		[SetUp]
		public void SetUp()
		{
			_subject = new ObjectSerializer();
		}

		[Test]
		public void ObjectSerializationTest()
		{
			var expected = new SimpleType { TextA = "textA", TextB = "hello" };

			byte[] data;
			using (MemoryStream outputStream = new MemoryStream())
			{
				_subject.Serialize(outputStream, expected);
				data = outputStream.ToArray();
			}
			using (MemoryStream inputStream = new MemoryStream(data))
			{
				var deserialized = _subject.Deserialize<object>(inputStream);
				Assert.That(deserialized, Is.EqualTo(expected));
			}
		}

		[Test]
		public void ConsecutiveSerializationTest()
		{
			var expected1 = new SimpleType { TextA = "textA", TextB = "hello" };
			var expected2 = new ComplexStruct { Span = TimeSpan.FromMilliseconds(56), Text = "test" };

			byte[] data;
			using (MemoryStream outputStream = new MemoryStream())
			{
				_subject.Serialize(outputStream, expected1);
				_subject.Serialize(outputStream, expected2);
				data = outputStream.ToArray();
			}
			using (MemoryStream inputStream = new MemoryStream(data))
			{
				var deserialized1 = _subject.Deserialize<object>(inputStream);
				var deserialized2 = _subject.Deserialize<object>(inputStream);
				Assert.That(deserialized1, Is.EqualTo(expected1));
				Assert.That(deserialized2, Is.EqualTo(expected2));
			}
		}
	}
}
