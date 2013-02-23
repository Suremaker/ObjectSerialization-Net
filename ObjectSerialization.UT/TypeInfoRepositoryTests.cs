using System;
using NUnit.Framework;
using ObjectSerialization.Types;
using ObjectSerialization.UT.Helpers;

namespace ObjectSerialization.UT
{
	[TestFixture]
	public class TypeInfoRepositoryTests
	{
		[Test]
		public void BasicTypesShouldBeRegisteredAsPredefined()
		{
			AssertRegistrationFor(typeof(string));
			AssertRegistrationFor(typeof(bool));
			AssertRegistrationFor(typeof(byte));
			AssertRegistrationFor(typeof(sbyte));
			AssertRegistrationFor(typeof(ushort));
			AssertRegistrationFor(typeof(short));
			AssertRegistrationFor(typeof(uint));
			AssertRegistrationFor(typeof(int));
			AssertRegistrationFor(typeof(ulong));
			AssertRegistrationFor(typeof(long));
			AssertRegistrationFor(typeof(char));
			AssertRegistrationFor(typeof(float));
			AssertRegistrationFor(typeof(double));
			AssertRegistrationFor(typeof(decimal));
			AssertRegistrationFor(typeof(object));
			AssertRegistrationFor(typeof(DateTime));
			AssertRegistrationFor(typeof(TimeSpan));
			AssertRegistrationFor(typeof(Guid));
		}

		[Test]
		public void ShouldRegisterAllSerializableTypesFromAssemblyInProperOrder()
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(GetType().Assembly);
			AssertRegistrationFor(typeof(SerializableType));
			AssertRegistrationFor(typeof(InternalSerializableType));
			Assert.That(
				TypeInfoRepository.GetTypeInfo(typeof(InternalSerializableType)).ShortTypeId,
				Is.LessThan(TypeInfoRepository.GetTypeInfo(typeof(SerializableType)).ShortTypeId));
		}

		private void AssertRegistrationFor(Type type)
		{
			AssertTypeIsRegistered(type);
			AssertTypeIsRegistered(type.MakeArrayType());
		}

		private static void AssertTypeIsRegistered(Type type)
		{
			Assert.That(TypeInfoRepository.GetTypeInfo(type).ShortTypeId, Is.Not.Null, type.FullName + " is not registered as predefined type!");
		}
	}
}
