using System;
using System.Collections;
using System.Collections.Generic;
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
	class ItermediatePerformanceOptimizations
	{
		static void DoSomething(object obj = null)
		{
			// this method do almost nothing
		}

		static byte GetByte(int i) => (byte) (i / 1000);

		/* Gen-0 optimizations
			The more objects in generation 0, the more work GC has to do. So:
			- Limit the number of objects you create - be sure there is no redundant object
			- Allocate, use, and discard objects as quickly as possible so they are all ready to be deallocated in the next GC cycle
	    */
		public static void FastGarbageCollection()
	    {
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

			Console.WriteLine();
	    }
	}
}
