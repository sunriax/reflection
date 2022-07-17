using AbstractReflectorLib;
using IReflectorLib;
using RaGae.ReflectionLib;
using System;
using System.IO;

namespace MakeReflection
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] concreteFiles;

            // +--------------------+
            // | Abstract class     |
            // +--------------------+

            // Config mode (config that contains data)
            Reflection r1 = new Reflection(@"ReflectionLib.json", 0);

            AbstractReflector a1 = r1.GetInstanceByProperty<AbstractReflector>("Key", "Lib1");
            AbstractReflector b1 = r1.GetInstanceByProperty<AbstractReflector>("Key", "Lib1", new object[] { "Injected constructor parameter" });
            Console.WriteLine(a1.Message());
            Console.WriteLine(b1.Message());

            // Directory mode (directory that contains libraries)
            Reflection r2 = new Reflection(@"Reflection", "*ReflectorLib.dll");

            AbstractReflector a2 = r2.GetInstanceByProperty<AbstractReflector>("Key", "Lib1");
            AbstractReflector b2 = r2.GetInstanceByProperty<AbstractReflector>("Key", "Lib1", new object[] { "Injected constructor parameter" });
            Console.WriteLine(a2.Message());
            Console.WriteLine(b2.Message());

            // File mode (path to libraries)
            concreteFiles = Directory.GetFiles(@"Reflection");

            Reflection r3 = new Reflection(concreteFiles);

            AbstractReflector a3 = r3.GetInstanceByProperty<AbstractReflector>("Key", "Lib1");
            AbstractReflector b3 = r3.GetInstanceByProperty<AbstractReflector>("Key", "Lib1", new object[] { "Injected constructor parameter" });
            Console.WriteLine(a3.Message());
            Console.WriteLine(b3.Message());


            // +--------------------+
            // | Interface          |
            // +--------------------+

            // Config mode (config that contains data)
            Reflection r4 = new Reflection(@"ReflectionLib.json", 1);

            IReflector a4 = r4.GetInstanceByProperty<IReflector>("Key", "Lib1");
            IReflector b4 = r4.GetInstanceByProperty<IReflector>("Key", "Lib1", new object[] { "Injected constructor parameter" });
            Console.WriteLine(a4.Message());
            Console.WriteLine(b4.Message());

            // Directory mode (directory that contains libraries)
            Reflection r5 = new Reflection(@"IReflection", "*IReflectorLib.dll");

            IReflector a5 = r5.GetInstanceByProperty<IReflector>("Key", "Lib1");
            IReflector b5 = r5.GetInstanceByProperty<IReflector>("Key", "Lib1", new object[] { "Injected constructor parameter" });
            Console.WriteLine(a5.Message());
            Console.WriteLine(b5.Message());

            // File mode (path to libraries)
            concreteFiles = Directory.GetFiles(@"IReflection");

            Reflection r6 = new Reflection(concreteFiles);

            IReflector a6 = r6.GetInstanceByProperty<IReflector>("Key", "Lib1");
            IReflector b6 = r6.GetInstanceByProperty<IReflector>("Key", "Lib1", new object[] { "Injected constructor parameter" });
            Console.WriteLine(a6.Message());
            Console.WriteLine(b6.Message());

            Console.ReadKey();
        }
    }
}
