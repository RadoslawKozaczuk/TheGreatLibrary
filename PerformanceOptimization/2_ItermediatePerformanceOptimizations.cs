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

	    /* Gen-0 optimizations
			The more objects in generation 0, the more work GC has to do. So:
			- Limit the number of objects you create - be sure there is no redundant object
			- Allocate, use, and discard objects as quickly as possible so they are all ready to be deallocated in the next GC cycle
	    */
	    public static void ImmutableStringExample()
	    {
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


			// this is another garbage code
		    var list = new ArrayList();
		    for (int i = 0; i < 10000; i++)
		    {
			    list.Add(i);
		    }

			// it can be rewritten to this beauty
		    var betterList = new List<int>(10000);
		    for (int i = 0; i < 10000; i++)
		    {
			    betterList.Add(i);
		    }
			// by using generic list we avoid using internal object array and use int array instead
			// this allows use to avoid boxing and unboxing
			// it results in 20000 less boxed integer objects on the heap
			// additionally it is allocated with a precise size which allows use to avoid reallocation


			// and the last one
			var candy = new Dictionary<int, int>();
			// [...] // tons of code that will certainly be executed long enough to run GC cycle effectively making our candy to stuck in gen-2 forever
			DoSomething(candy); // usage of candy

			// here is how it suppose to look like
		    var betterCandy = new Dictionary<int, int>();
		    DoSomething(betterCandy); // usage of candy
		    betterCandy = null;
			// [...] // tons of code that no one cares about anymore

		

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




			Console.WriteLine();
	    }
	}
}
