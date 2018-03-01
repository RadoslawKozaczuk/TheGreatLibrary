using System;
using System.Diagnostics;

/*
	Stack:
	- Tracks method calls
	- Contains frames which hold parameters, return address and local variables for each method call
	- A stack frame is removed when returning from a method. All local variables go out of scope at this point
	- If you call too many methods, the stack will fill up completely and .Net Framework throws 
		a StackOverflow Exception. This usually happens when a method call itself somewhere in the code
		
	Heap:
	- The new keyword creates objects on the heap
	- Code that uses objects on the heap is slightly slower than code using integers on the stack
	- When variables on the stack go out of the scope, their corresponding objects on the heap are dereferenced, not destroyed
	- The .NET Framework will always postpone cleaning up dereferenced objects
	- The .NET Framework eventually stats a process called Garbage Collection and deallocates all dereferenced objects on the heap
	- Every static variable is stored on the heap, regardless of whether it's declared within a reference type or a value type

	The same code that uses integers on the stack is slightly faster than code that uses objects on the heap.

	Value types:
	- Value type can exists on the stack and on the heap
	- Value types are assigned by value so a = b means value of b is copied over to a
	- Value types are compared by value meaning a == b is true when value of a and value of b are equal

	Reference types:
	- Reference types can be set to null
	- Reference type store a reference to a value on the heap
	- Reference types can exist on the stack and the heap
	- Reference types are assigned by reference
	- Reference types are compared by reference

	Boxing and unboxing:
	- Boxing takes a value type on the stack and stores it as an object on the heap
	- Boxing happens when you assign a value type to a variable, parameter, field or property of type object
	- Unboxing unpacks a boxed object on the heap, and copies the value type inside back to the stack
	- Unboxing happens when you cast an object value a value type
	- Boxing and unboxing negatively affect performance
*/

namespace PerformanceOptimization
{
    class Program
    {
		/*
			Immutable string
			- Strings is a reference types, and immutable
			- Strings behave as if they are value types. They are assigned and compared by value
			- Strings are thread-safe
		*/
	    public static void ImmutableStringExample()
	    {
		    string a = "abc";
		    string b = a; // at the moment both references point at the same value
		    b += "d"; // modifying a string results in the creation of a new string, leaving the original string unchanged

		    /*
				So string are reference types but when modified they are copied
				Thats why they are called immutable strings
				Benefits of such solution:
				- Immutable string are thread-safe
				- Identical strings can be merged (memory saving)
				- copy a string by copying the reference (fast assignment)
				- compare two strings by comparing the reference
	        */

			Console.WriteLine("b = " + b);
	    }

	    /*
			2-step compilation - .Net Framework first compiles to CIL language and this is what is stored in .dll and .exe files.
			When we run the .exe file Just-In-Time (JIT) compiler kicks in and compile our code to machine language.

			Advantages:
			- JIT compiler can optimize for specific CPU
			- Creates portable code that runs on any platform
			- Code can be verified before running
			- Code can be annotated with attributes

			Disadvantages:
			- Slightly slower code - at this moment compilators are so advanced 
				that it is almost impossible to hand made better code than what compilers produces

			Intermediate Language concepts:
			1) IL instructions - sequence of instructions, executed one by one.
			2) Local variable locations - slots for variables
			3) Evaluation stack - have only two operations push and pop which add or removes the 

			We have only 3 types of instructions - pushing on the stack, removing from the stack, and modifying value on the stack.

			Example:
			int i = 456;
			i = i + 1;

			CIL:
			ldc.i4.1 - Loads the 4-byte signed integer constant 1 on the evaluation stack
			ldloc.0 - Load the variable in location 0 on the evaluation stack. The other value on the stack is pushed down
			add - Add the top two numbers on the evaluation stack together, and replace them with the result
			stloc.0 - Remove the value at the top of the evaluation stack, and store it in location 0
		*/
		public static int BasicCilCode()
	    {
		    int i = 456;
			i = i + 1;
		    return i;

			/* translated
	        IL_0000: nop // Do nothing (No operation); I have no idea what it is for...
		    
	        IL_0001: ldc.i4       456	
	        IL_0006: stloc.0      // i
		    
	        IL_0007: ldloc.0      // i
	        IL_0008: ldc.i4.1
	        IL_0009: add
	        IL_000a: stloc.0      // i
		    
			IL_000b: ldloc.0      // i
			IL_000c: stloc.1      // V_1
			IL_000d: br.s         IL_000f	// Branch to target, short form.

	        IL_000f: ldloc.1      // V_1
			IL_0010: ret   
		    */
		}

	    private const int ArraySize = 1000000;

	    public static long MeasureA()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int a = 1;
		    for (int i = 0; i < ArraySize; i++)
		    {
			    a = a + 1;
			    // this line compiles to:
				//ldloc.1      // a
				//ldc.i4.1	 // constant 1
				//add
				//stloc.1      // a

			}
			stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

	    public static long MeasureB()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    object a = 1;
		    for (int i = 0; i < ArraySize; i++)
		    {
			    a = (int)a + 1;
				// this line compiles to:
			    //ldloc.1      // a
			    //unbox.any		[System.Runtime]System.Int32
			    //ldc.i4.1
			    //add
			    //box	[System.Runtime]System.Int32
			    //stloc.1      // a
			}
		    stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

		public static void AvoidingBoxing()
	    {
		    // 1st run to eliminate any startup overhead
		    MeasureA();
		    MeasureB();

		    // measurement run
		    long intDuration = MeasureA();
		    long objDuration = MeasureB();

		    // display results
		    Console.WriteLine("Integer performance: {0} milliseconds", intDuration);
		    Console.WriteLine("Object performance: {0} milliseconds", objDuration);
		    Console.WriteLine();
		    Console.WriteLine("Method B is {0} times slower", 1.0 * objDuration / intDuration);
			
			/* Summary 
			- Casting object variables to value types introduces an UNBOX instruction in intermediate code
			- Storing value types in object variables introduces a BOX instruction in intermediate code
			- We should avoid casting to and from objects in critical-performance code.
			- Additionally System.Collections and System.Collections.Specialized should also be avoided 
				because internally they use object arrays and therefore force boxing-unboxing operations.
			- Same goes to System.Data classes - DataRow uses object arrays internally for storing data
			- Even typed data sets are not ok because what they do internally is object casting
			- But, on the other hand System.Collections.Generic of type T are ok
			*/
		}

		static void Main()
        {
			//Console.WriteLine("i = " + BasicCilCode());

	        AvoidingBoxing();
			Console.ReadLine();
        }
    }
}