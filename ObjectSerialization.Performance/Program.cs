using System;
using System.Collections.Generic;
using System.IO;
using ObjectSerialization.Performance.Results;
using ObjectSerialization.Performance.Serializers;
using ObjectSerialization.Performance.TestCases;
using ObjectSerialization.Performance.TestObjects;
using ObjectSerialization.Types;

namespace ObjectSerialization.Performance
{
	class Program
	{
		static void Main(string[] args)
		{
			var monitor = new PerformanceMonitor(
				new ObjectSerializerAdapter(),
				new BinaryFormatterAdapter(),
				new ProtoBufAdapter());

			PerformMeasurement(monitor);

			string date = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
			WriteResult(date, new CsvResultPresenter(monitor.GetResults()));
			WriteResult(date, new HtmlResultPresenter(monitor.GetResults()));

			Console.WriteLine("Done...");
			Console.ReadKey();
		}

	    private static void PerformMeasurement(PerformanceMonitor monitor)
		{
			monitor.MeasureFor(Case.For(new SimpleClass { Number = 32, Double = 3.14, Text = "test" }));
			monitor.MeasureFor(Case.For("SimpleClass null text", new SimpleClass { Number = 32, Double = 3.14 }));

			monitor.MeasureFor(Case.For(new[]
            {
                new SimpleClass {Number = 32, Double = 3.14, Text = "test1"},
                new SimpleClass {Number = 5, Double = 7.14, Text = "test2"},
                new SimpleClass {Number = -3, Double = 21.14, Text = "test3"}
            }));

			monitor.MeasureFor(Case.For(new BasicTypes
			{
				Bool = true,
				Byte = 32,
				Character = 'a',
				Decimal = 55,
				Double = 3.2,
				Float = 2.3f,
				Int = 3,
				Long = 4,
				SByte = 2,
				Short = 33,
				String = "some text",
				UInt = 33,
				ULong = 66,
				UShort = 22
			}));

			monitor.MeasureFor(Case.For(new ComplexType
			{
				Date = DateTime.Now,
				NullableInt = 35,
				InterfaceHolder = new Impl { Text = "test" },
				ObjectHolder = new SimpleClass { Double = 32 },
				BaseType = new Derived { Value = 65.5, Other = 'c' }
			}));

			monitor.MeasureFor(Case.For(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
			monitor.MeasureFor(Case.For(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
			monitor.MeasureFor(Case.For(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
			monitor.MeasureFor(Case.For(new[] { "a", "b", "c", "d", "e", "other" }));

			monitor.MeasureFor(Case.For(new StructureHolder
			{
				Date = DateTime.Now,
				Span = TimeSpan.FromSeconds(333),
				Pair = new KeyValuePair<DateTime, decimal>(DateTime.UtcNow, 55),
				Guid = Guid.NewGuid(),
				Custom = new CustomStruct(66)
			}));

			monitor.MeasureFor(Case.For("Mixed array", new object[] { 55, 3.15, null, "test", DateTime.Now, new SimpleClass { Text = "test" } }));

			monitor.MeasureFor(Case.For("List[SimpleClass]", new List<SimpleClass>
            {
                new SimpleClass {Double = 5, Number = 32, Text = "a"},
                new SimpleClass {Double = 45, Number = 332, Text = "bb"}
            }));

			monitor.MeasureFor(Case.For("Dictionary[int,string]", new Dictionary<int, string> { { 3, "3" }, { 55, "fifty five" } }));

			var linkedList = new LinkedList<SimpleClass>();
			linkedList.AddLast(new SimpleClass { Double = 5, Number = 32, Text = "a" });
			linkedList.AddLast(new SimpleClass { Double = 45, Number = 332, Text = "bb" });
			monitor.MeasureFor(Case.For("LinkedList[SimpleClass]", linkedList));


			monitor.MeasureFor(Case.For(new PolymorphicHolder
			{
				A = new Derived { Other = 'c', Value = 34.2 },
				B = new Derived2 { Other = 55, Value = 34.2 }
			}));

			monitor.MeasureFor(Case.For<object>("SimpleClass as object", new SimpleClass { Number = 32, Double = 3.14, Text = "test" }));

			var hugeObject = new KeyValuePair<StructureHolder, BasicTypes>[25];
			for (int i = 0; i < hugeObject.Length; ++i)
				hugeObject[i] = new KeyValuePair<StructureHolder, BasicTypes>(new StructureHolder(), new BasicTypes());

			monitor.MeasureFor(Case.For("huge object", hugeObject));


			monitor.MeasureFor(Case.For("object array", new object[] { new SimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new SimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new SimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new SimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new SimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue } }));

			TypeInfoRepository.RegisterPredefined(typeof(RegisteredSimpleClass));
			monitor.MeasureFor(Case.For("object array with registered type", new object[] { new RegisteredSimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new RegisteredSimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new RegisteredSimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new RegisteredSimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue }, new RegisteredSimpleClass { Text = "test", Double = Double.MaxValue, Number = int.MaxValue } }));

		}

	    private static void WriteResult(string date, ResultPresenter presenter)
	    {
	        File.WriteAllText(string.Format("results {0}.{1}", date, presenter.Extension), presenter.Present());
	    }
	}
}
