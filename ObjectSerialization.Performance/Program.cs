using System;
using System.IO;
using ObjectSerialization.Performance.Serializers;
using ObjectSerialization.Performance.TestCases;
using ObjectSerialization.Performance.TestObjects;

namespace ObjectSerialization.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var monitor = new PerformanceMonitor(new DCSerializer(), new BFSerializer());

            monitor.MeasureFor(Case.For(new SimpleClass { Number = 32, Double = 3.14, Text = "test" }));
            monitor.MeasureFor(Case.For("SimpleClass null text", new SimpleClass { Number = 32, Double = 3.14 }));
            monitor.MeasureFor(Case.For(new[]
            {
                new SimpleClass{ Number = 32, Double = 3.14,Text = "test1"},
                new SimpleClass{ Number = 5, Double = 7.14,Text = "test2"},
                new SimpleClass{ Number = -3, Double = 21.14,Text = "test3"},
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

            File.WriteAllText(string.Format("results {0}.csv", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss")), monitor.GetResults());

            Console.ReadKey();
        }
    }
}
