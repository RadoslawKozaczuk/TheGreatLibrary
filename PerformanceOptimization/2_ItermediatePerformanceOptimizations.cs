using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace PerformanceOptimization
{
	/* .Net Framework Garbage Collector
		- The Garbage Collector is a generational garbage collector that uses a "mark, sweep/compact" cycle
		- Small objects (<85kb) are allocated on the Small Object Heap (SOH) which is generational (3 separated heaps gen-0, gen-1, gen-2)
		- Large objects (>85kb) are allocated on the Large Object Heap (LOH) which has only one generation (gen-2)
		- The flow looks like this:
			1) iterate through all objects and mark these that are dereferenced
			2) iterate through all marked objects and deallocate
			3) compact heaps and move them to the next heap
		- Gen-1 is collected frequently, gen-1 and gen-2 (SOH and LOH) much less so. The GC assumes that whatever reach gen-1 and 2
			must be long living objects.
	
		Why two heaps?
		Two successive garbage collection cycles will move and compact a long-living twice meaning 4 memcopy operations for a single object.
		For very large object that would impact the performance. That's why large object are not moved or compacted.
	
		The algorithm is based on two assumptions:
		- 90% of all small objects are short-lived
		- all large objects are long-lived
	 */
	public class ItermediatePerformanceOptimizations
	{
		// benchmarking constants
		const int Million = 1_000_000;
		const int HundredThousand = 100_000;
		const int Hundred = 100;

		static void DoSomething(object obj = null) { /* image this method doing something small */ }

		static byte GetByte(int i) => (byte) (i / 1000);

		/* Gen-0 optimizations
			The more objects in generation 0, the more work GC has to do. So:
			- Limit the number of objects you create - be sure there is no redundant object
			- Allocate, use, and discard objects as quickly as possible so they are all ready to be deallocated in the next GC cycle
	    */
		public static void FastGarbageCollection()
	    {
		    Console.WriteLine("This example does not provide any output, please check the code.");

			// Example 1:
			// this is example of a shit code
			var s = new StringBuilder();
		    for (int i = 0; i < 10000; i++)
		    { 
			    s.Append(i + "KB");
		    }
			
		    // it can be rewritten to this beauty
		    for (int i = 0; i < 10000; i++)
		    {
			    s.Append(i);
			    s.Append("KB");
		    }
			// strings are immutable so every concatenation creates a new object on the heap
			// in this case it means 40000 less objects on the heap!

			
			// Example 2:
			// this is another garbage code
		    var list = new ArrayList();
		    for (int i = 0; i < 10000; i++)
			    list.Add(i);
			
		    // it can be rewritten to this beauty
		    var betterList = new List<int>(10000);
		    for (int i = 0; i < 10000; i++)
			    betterList.Add(i);
			// by using generic list we avoid using internal object array and use int array instead
			// this allows use to avoid boxing and unboxing
			// it results in 20000 less boxed integer objects on the heap
			// additionally it is allocated with a precise size which allows use to avoid reallocation

			
			// Example 3:
			var candy = new Dictionary<int, int>();
			// [...] // tons of code that will certainly be executed long enough to run GC cycle effectively making our candy to stuck in gen-2 forever
			DoSomething(candy); // usage of candy
			
		    // here is how it suppose to look like
		    var betterCandy = new Dictionary<int, int>();
		    DoSomething(betterCandy); // usage of candy
		    betterCandy = null;
			// [...] // tons of code that no one cares about anymore

		
			// Example 4:
			// object pooling is an idea to use the same object over and over again
			var list2 = new ArrayList(85190);

			// look at this totally ugly code that dares to not use object pooling
		    DoSomething(list2);
			// [...] // tons of code that will certainly be executed long enough to run GC cycle
			list2 = new ArrayList(85190);
		    DoSomething(list2);
			
			// here is how it suppose to look like
		    DoSomething(list2);
			// [...] // tons of code that will certainly be executed long enough to run GC cycle
			list2.Clear();
		    DoSomething(list2);
			// this change increase the performance and reduce the change the LOH will become fragmented


			// Example 5:
			var list3 = new ArrayList(85190);
		    for (int i = 0; i < 10000; i++)
		    {
			    list3.Add(new KeyValuePair<int,int>(i, i + 1));
		    }

			// the problem with the above is that ArrayList is a large object
			// but it is filled with tiny objects that goes on Small Object Heap
			// and because the list keeps reference to each of them none will ever dereference
			// so all objects will eventually go to gen-2
			var list_1 = new int[85190];
			var list_2 = new int[85190];
		    for (int i = 0; i < 10000; i++)
		    {
			    list_1[i] = i;
			    list_2[i] = i + 1;
		    }
			// thanks to this we now have two list and nothing on the heap


		    // Example 6 - Converting a large short-lived object into a small short-lived object.
		    // Splitting objects. Reducing object foot print.

			// generally the Garbage Collector assumes 90% of all small objects are short-lived, and all large objects are long-lived. 
			// So we should avoid large short-lived objects and small long-lived objects.
			// int has 32 bits so 4 bytes regardless the CPU architecture

			// evil code
			var buffer = new int[32768]; // this adds up to 128 thousand bytes. This is above the LO's threshold.
		    // and therefore it goes directly to LOH and gets collected during generation 2.
		    for (int i = 0; i < buffer.Length; i++)
		    {
			    buffer[i] = GetByte(i);
		    }

		    // good code
		    var betterBuffer = new byte[32768]; // this is stored on the SOH and therefore is managed more efficiently.
		    for (int i = 0; i < buffer.Length; i++)
		    {
			    betterBuffer[i] = GetByte(i);
		    }

			
			// Example 7 - Converting a small long-lived object to a large long-lived object.
			// Merge objects. Resize lists.

			// bad code
			// public static ArrayList staticList = new ArrayList();
			// [...] // lots of other code
			//DoSomething(staticList);

			// good code - it is clear that the object was intended to be long-lived if it was made static
			// public static ArrayList staticList = new ArrayList(85190); - presizing it makes sure it will go to generation 2. So we avoid merging compacting etc.
		    // [...] // lots of other code
		    //DoSomething(staticList);
	    }

		
		delegate void AddDelegate(int a, int b, out int result);
		static void Add1(int a, int b, out int result) => result = a + b;
		static void Add2(int a, int b, out int result) => result = a + b;

		// Measure1: call Add1 and Add2 manually
		static long DelegateMeasure1()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < HundredThousand; i++)
			{
				Add1(1234, 2345, out _);
				Add2(1234, 2345, out _);
			}
			sw.Stop();
			return sw.ElapsedTicks;
		}

		// Measure2: call Add1 and Add2 using 2 unicast delegates
		static long DelegateMeasure2()
		{
			AddDelegate add1 = Add1;
			AddDelegate add2 = Add2;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < HundredThousand; i++)
			{
				add1(1234, 2345, out _);
				add2(1234, 2345, out _);
			}
			sw.Stop();
			return sw.ElapsedTicks;
		}

		// Measure3: call Add1 and Add2 using 1 multicast delegate
		static long DelegateMeasure3()
		{
			// multicast delegate is just a delegate that has many methods assign
			// usage is exactly the same and methods are called in the sequence in order in which they have been added
			AddDelegate multiAdd = Add1;
			multiAdd += Add2;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < HundredThousand; i++)
			{
				multiAdd(1234, 2345, out _);
			}
			sw.Stop();
			return sw.ElapsedTicks;
		}

		// if the code is performance critical we should avoid using this trick
		delegate void TrickDelegate();
		static void TrickMethod() { }
		static TrickDelegate GetDelegateOrMaybeNull() => null;
		static void NeatTrickButSlow()
		{
			TrickDelegate trick = () => { }; // preinitialize with a dummy lambda to avoid null checking
			trick += GetDelegateOrMaybeNull();
			trick(); // we don't need to check for null

			// but it is slower - better use this
			TrickDelegate trick2 = GetDelegateOrMaybeNull();
			if (trick2 != null)
				trick();
		}
		
		public static void FastDelegates()
		{
			long manual = 0;
			long unicast = 0;
			long multicast = 0;
			for (int i = 0; i < Hundred; i++)
			{
				manual += DelegateMeasure1();
				unicast += DelegateMeasure2();
				multicast += DelegateMeasure3();
			}
			Console.WriteLine($"Manual calls: {manual} ticks");
			Console.WriteLine($"Unicast delegates: {unicast} ticks");
			Console.WriteLine($"Multicast delegate: {multicast} ticks");

			/* Results:
				- direct call is the fastest method
				- unicast delegates are slightly slower
				- multicast delegates are up more than two times slower

				Basically if you want performance don't use delegates
			*/
		}


		// a delegate to create the object
		delegate object ClassCreator();

		// dictionary to cache class creators
		static readonly Dictionary<string, ClassCreator> _classCreators = new Dictionary<string, ClassCreator>();

		static long FactoryHardcodedMeasure(string typeName)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Million; i++)
			{
				switch (typeName)
				{
					case "System.Text.StringBuilder":
						new StringBuilder();
						break;
					default:
						throw new NotImplementedException();
				}
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		// this is flexible but really slow
		static long FactoryReflectionMeasure(string typeName)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Million; i++)
				Activator.CreateInstance(Type.GetType(typeName));

			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		/// <summary>
		/// Creates a class by using Intermediate Language 
		/// 1. Check dictionary if the delegate has been created already
		/// 2. If so -> retrieve and call delegate
		/// 3. If not ->
		///		4. Create dynamic method and write newObj and ret instructions into it
		///		5. Wrap method in a delegate and store in dictionary
		///		6. Call delegate to instantiate class
		/// </summary>
		static ClassCreator GetClassCreator(string typeName)
		{
			// get delegate from dictionary
			if (_classCreators.ContainsKey(typeName))
				return _classCreators[typeName];
			
			// get the default constructor of the type
			var type = Type.GetType(typeName);
			ConstructorInfo ctorInfo = type.GetConstructor(new Type[0]);

			// create a new dynamic method that constructs and returns the type
			string methodName = type.Name + "Ctor";
			var dynamicMethod = new DynamicMethod(methodName, type, new Type[0], typeof(object).Module);
			var ilGenerator = dynamicMethod.GetILGenerator();
			ilGenerator.Emit(OpCodes.Newobj, ctorInfo);
			ilGenerator.Emit(OpCodes.Ret);

			// add delegate to dictionary and return
			var creator = (ClassCreator)dynamicMethod.CreateDelegate(typeof(ClassCreator));
			_classCreators.Add(typeName, creator);

			// return a delegate to the method
			return creator;
		}
		
		static long FactoryDelegateMeasure(string typeName)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Million; i++)
			{
				var classCreator = GetClassCreator(typeName);
				classCreator();
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		public static void ClassFactories()
		{
			// A class factory is a special class that constructs other classes on demand, based on external configuration data.
			// for example:
			// var conn = ConnectionFactory.GetConnection("Some DB");
			
			// measurement run
			long duration1 = FactoryHardcodedMeasure("System.Text.StringBuilder");
			long duration2 = FactoryReflectionMeasure("System.Text.StringBuilder");
			long duration3 = FactoryDelegateMeasure("System.Text.StringBuilder");

			Console.WriteLine($"Compile-time construction: {duration1}");
			Console.WriteLine($"Dynamic construction: {duration2}");
			Console.WriteLine($"CIL method construction: {duration3}");
			
			/* Results
				- Activator is around one hundred times slower 
				- Dynamic method delegates are 5 times slower than compiled code
			 */
		}
	}
}