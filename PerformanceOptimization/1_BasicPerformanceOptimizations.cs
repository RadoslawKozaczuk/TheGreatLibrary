using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
    public static class BasicPerformanceOptimizations
    {
	    const int TenMillion = 10_000_000;
		const int Million = 1_000_000;
	    const int TenThousand = 10_000;
	    const int Thousand = 1_000;
	    const int Hundert = 100;

		/* Immutable string
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

		/* Common Intermediate Language
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
		
		static long BoxMeasureA()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			int a = 1;
			for (int i = 0; i < Million; i++)
			{
				a = a + 1;
				/* this line compiles to:
					ldloc.1      // a
					ldc.i4.1	 // constant 1
					add
					stloc.1      // a
				*/
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long BoxMeasureB()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			object a = 1;
			for (int i = 0; i < Million; i++)
			{
				a = (int)a + 1;
				/* this line compiles to:
					ldloc.1      // a
					unbox.any		[System.Runtime]System.Int32
					ldc.i4.1
					add
					box	[System.Runtime]System.Int32
					stloc.1      // a
				*/
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		public static void AvoidingBoxing()
		{
			// measurement run
			long intDuration = BoxMeasureA();
			long objDuration = BoxMeasureB();

			// display results
			Console.WriteLine($"Integer performance: {intDuration} milliseconds");
			Console.WriteLine($"Object performance: {objDuration} milliseconds");
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
		
		static long StringConcatMeasureA()
		{
			var s = string.Empty;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenThousand; i++)
			{
				s = s + "a";
			}
			stopwatch.Stop();
			return stopwatch.ElapsedTicks;
		}

		static long StringConcatMeasureB()
		{
			var sb = new StringBuilder();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenThousand; i++)
			{
				sb.Append("a");
			}
			stopwatch.Stop();
			return stopwatch.ElapsedTicks;
		}

	    static long NormalStringConcat(int additions)
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    for (int i = 0; i < Million; i++)
		    {
			    var s = string.Empty;
			    for (int j = 0; j < additions; j++)
			    {
				    s = s + "a";
			    }
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

	    static long StringBuilderStringConcat(int additions)
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    for (int i = 0; i < Million; i++)
		    {
			    var sb = new StringBuilder();
			    for (int j = 0; j < additions; j++)
			    {
				    sb.Append("a");
			    }
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

		public static void UsingStringBuilder()
		{
			// measurement run
			long duration1 = StringConcatMeasureA();
			long duration2 = StringConcatMeasureB();

			// display results
			Console.WriteLine($"Number of concatenations: {TenThousand}");
			Console.WriteLine($"String performance: {duration1} ticks");
			Console.WriteLine($"StringBuilder performance: {duration2} ticks");

			Console.WriteLine();
			Console.WriteLine("Overall performance per number of concatenations:");
			for (int reps = 0; reps < 10; reps++)
			{
				long duration3 = NormalStringConcat(reps);
				long duration4 = StringBuilderStringConcat(reps);
				Console.WriteLine($"Reps: {reps + 1}, String: {duration3}, StringBuilder: {duration4}");
			}
			// regular string concatenation is more efficient up to 4 additions,
			// after this point we should avoid using "+" operator and start using the StringBuilder
			// in huge numbers the StringBuilder is massively faster
		}

		static long ArrayListPerformanceDynamic()
		{
			var list = new ArrayList();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				list.Add(i);
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long ArrayListPerformancePresized()
		{
			var list = new ArrayList(TenMillion);
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				list.Add(i);
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long GenericListPerformanceDynamic()
		{
			var list = new List<int>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				list.Add(i);
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long GenericListPerformancePresized()
		{
			var list = new List<int>(TenMillion);
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				list.Add(i);
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long ArrayPerformance()
		{
			var list = new int[TenMillion];
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				list[i] = i;
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		public static void CollectionPerformanceExample()
		{
			// measurement run
			long duration1 = ArrayListPerformanceDynamic();
			long duration2 = ArrayListPerformancePresized();
			long duration3 = GenericListPerformanceDynamic();
			long duration4 = GenericListPerformancePresized();
			long duration5 = ArrayPerformance();

			Console.WriteLine($"ArrayList with overflow: {duration1} milliseconds");
			Console.WriteLine($"ArrayList presized: {duration2} milliseconds");
			Console.WriteLine($"List<int> with overflow: {duration3} milliseconds");
			Console.WriteLine($"List<int> presized: {duration4} milliseconds");
			Console.WriteLine($"int[]: {duration5} milliseconds");

			/*
				- Always use the generic collection classes from the System.Collection.Generic namespace in performance critical code
				- Avoid the classes in the System.Collection namespace in performance critical code
				- presizing the list boost the performance
				- there is not much difference between List<int> and int[] but all in all int[] is faster
			*/
		}
		
	    static long OneDimArray()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int[] list = new int[TenThousand * TenThousand];
		    for (int i = 0; i < TenThousand * TenThousand; i++)
		    {
			    list[i] = i;
				/* compiles to
					ldloc.1      // list
					ldloc.2      // i
					ldloc.2      // i
					stelem.i4 
				 */
			}
			stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

	    static long TwoDimArray()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int[,] list = new int[TenThousand, TenThousand];
		    for (int i = 0; i < TenThousand; i++)
		    {
			    for (int j = 0; j < TenThousand; j++)
			    {
				    list[i, j] = 1;
					/* compiles to
						ldloc.1      // list
						ldloc.2      // i
						ldloc.3      // j
						ldc.i4.1     
						call         instance void int32[0...,0...]::Set(int32, int32, int32)
					*/
				}
			}
		    stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

	    static long JaggedArray()
	    {
		    // it is array of arrays
		    // each sub array can have a different size hence the name - jagged array
			var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int[][] list = new int[TenThousand][];
		    for (int i = 0; i < TenThousand; i++)
		    {
			    list[i] = new int[TenThousand];
		    }
		    for (int i = 0; i < TenThousand; i++)
		    {
			    for (int j = 0; j < TenThousand; j++)
			    {
				    list[i][j] = 1;
					/* compiles to
						ldloc.1      // list
						ldloc.s      i_V_4
						ldelem.ref   
						ldloc.s      j
						ldc.i4.1     
						stelem.i4  
					*/
				}
			}
		    stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

	    static long FlattenedTwoDimArray()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int[] list = new int[TenThousand * TenThousand];
		    for (int i = 0; i < TenThousand; i++)
		    {
			    for (int j = 0; j < TenThousand; j++)
			    {
				    int index = TenThousand * i + j;
				    list[index] = 1;
			    }
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedMilliseconds;
	    }

		public static void ArraysExample()
	    {
			// measurement run
			long duration1 = OneDimArray();
		    long duration2 = JaggedArray();
		    long duration3 = TwoDimArray();
		    long duration4 = FlattenedTwoDimArray();

		    Console.WriteLine($"int[] (one-dim): {duration1} milliseconds");
		    Console.WriteLine($"int[,] (two-dim): {duration2} milliseconds");
		    Console.WriteLine($"int[][] (jagged): {duration3} milliseconds");
		    Console.WriteLine($"flattened two-dim: {duration4} milliseconds");

			// one dimension array is the fastest
			// Intermediate Language has native support for one dimensional array
			// that's why 1-dim comes first and jagged second
			// 2-dim array is just a class
			// although on my computer difference between the last two is marginal
			// their internal code also similar

			/* Summary
				- If you only have 1 dimension of data, use 1-dimensional arrays for the best performance
				- If you have 2 dimensions of data, flatten the array
				- If this is not possible, consider using a jagged array
				- If there is no other option, use a 2-dimensional array
			*/
		}

	    static long IncrementInteger()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int count = 0;
		    for (int i = 0; i < Hundert; i++)
		    {
			    count = count + 1;
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedTicks;
	    }

	    static long IncrementIntegerWithThrow()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    int count = 0;
		    for (int i = 0; i < Hundert; i++)
		    {
			    try
			    {
				    count = count + 1;
				    throw new InvalidOperationException();
			    }
			    catch (InvalidOperationException)
			    {
				    // there is no need to unwind the stack because the catch block is in the same place
					// there is no additional overhead of handling the exception
					// so this code will measure only the throwing overhead
				}
			}
		    stopwatch.Stop();
		    return stopwatch.ElapsedTicks;
	    }

	    public static void ExceptionsExample()
	    {
		    // measurement run
		    long duration1 = IncrementInteger();
		    long duration2 = IncrementIntegerWithThrow();

		    Console.WriteLine($"Normal: {duration1} ticks");
		    Console.WriteLine($"With exceptions: {duration2} ticks");

			// Exceptions adds massive overhead
			// in general we should never use exceptions in mission-critical code
	    }

	    // constants
	    private const int Digits = 5;

	    // fields
	    private static readonly char[] _digitArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'X' };
	    private static readonly List<string> _numbers = new List<string>();

	    static void PrepareList()
	    {
		    // this method prepares million element long list of random signs taken from the _digitArray
		    var random = new Random();
		    for (int i = 0; i < Hundert; i++)
		    {
			    var sb = new StringBuilder();
			    for (int d = 0; d < Digits; d++)
			    {
				    int index = random.Next(_digitArray.Length);
				    sb.Append(_digitArray[index]);
			    }
			    _numbers.Add(sb.ToString());
		    }
	    }

		static long ParsingWithException()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    for (int i = 0; i < Hundert; i++)
		    {
			    try
			    {
				    int.Parse(_numbers[i]);
			    }
			    catch (FormatException)
			    {
					// error is suppressed
			    }
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedTicks;
	    }

	    static long ParsingWithoutException()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    for (int i = 0; i < Hundert; i++)
		    {
				// try parse attempts to parse the number and simply do nothing if the parsing was unsuccessful - no exception thrown
				int.TryParse(_numbers[i], out int _); // name "_" is used to hide the unused variable as much as possible - it will not be usable
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedTicks;
	    }
		
	    public static void ParseVsTryParse()
	    {
		    // initialization
		    PrepareList();

		    // measurement run
		    long duration1 = ParsingWithException();
		    long duration2 = ParsingWithoutException();

		    Console.WriteLine($"int.Parse: {duration1} ticks");
		    Console.WriteLine($"int.TryParse: {duration2} ticks");

			// the difference is massive - order of magnitude is 5
		}
		
	    // fields
	    private static readonly List<int> _numbersV2 = new List<int>();
	    private static readonly Dictionary<int, string> _lookup = new Dictionary<int, string> {
		    { 0, "zero" },
		    { 1, "one" },
		    { 2, "two" },
		    { 3, "three" },
		    { 4, "four" },
		    { 5, "five" },
		    { 6, "six" },
		    { 7, "seven" },
		    { 8, "eight" },
		    { 9, "nine" }
	    };

	    static void PrepareListV2()
	    {
		    var random = new Random();
		    for (int i = 0; i < Hundert; i++)
			    _numbersV2.Add(random.Next(_lookup.Count + 1)); // 1 per 11 will be invalid
	    }

	    static long KeyNotFound()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    for (int i = 0; i < Hundert; i++)
		    {
			    string s = null;
			    try
			    {
				    s = _lookup[_numbersV2[i]];
			    }
			    catch (KeyNotFoundException) { }
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedTicks;
	    }

	    static long ContainsKey()
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();
		    for (int i = 0; i < Hundert; i++)
		    {
			    string s = null;
			    int key = _numbersV2[i];
			    if (_lookup.ContainsKey(key)) // this extra check avoids the exception
				    s = _lookup[key];
		    }
		    stopwatch.Stop();
		    return stopwatch.ElapsedTicks;
	    }

	    public static void ContainsKeyVsKeyNotFoundException()
	    {
		    // initialization
		    PrepareListV2();

		    // measurement run
		    long duration1 = ContainsKey();
		    long duration2 = KeyNotFound();

		    Console.WriteLine($"Lookup with ContainsKey: {duration1} ticks");
		    Console.WriteLine($"Lookup with KeyNotFoundException: {duration2} ticks");

			// like with the previous example additional computing overhead is MASSIVE!

			/* Summary:
				- Use exceptions for fatal conditions that require an abort
				- Do not put try-catch blocks in deeply nested code
				- Never use catch(Exception) to catch all exception because it will catch non fatal exceptions as well
				- Do not use exceptions for not-critical conditions
				- Never use exceptions to control the flow of your program
			 */
	    }
		
		// fields
		private static readonly ArrayList _arrayList = new ArrayList(TenMillion);
		private static readonly List<int> _genericList = new List<int>(TenMillion);
		private static readonly int[] _array = new int[TenMillion];
		
		static void PrepareListV3()
		{
			var random = new Random();
			for (int i = 0; i < TenMillion; i++)
			{
				int number = random.Next(256);
				_arrayList.Add(number);
				_genericList.Add(number);
				_array[i] = number;
			}
		}

		static long ArrayListForMeasure()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				int result = (int)_arrayList[i];
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long ArrayListForeachMeasure()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (int i in _arrayList)
			{
				int result = i;
				// there is a lot of unboxing involved which additionally slows down the loop
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long GenericListForMeasure()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				int result = _genericList[i];
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long GenericListForeachMeasure()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (int i in _genericList)
			{
				int result = i;
			}
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long ArrayForMeasure()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < TenMillion; i++)
			{
				int result = _array[i];
			}
			/* the loop compiles to:
				IL_0012: nop
				
				IL_0013: ldsfld int32[] PerformanceOptimization.BasicPerformanceOptimizations::_array
				IL_0018: ldloc.1      // i
				IL_0019: ldelem.i4
				IL_001a: stloc.2      // result
				
				IL_001b: nop
				
				IL_001c: ldloc.1      // i
				IL_001d: ldc.i4.1
				IL_001e: add
				IL_001f: stloc.1      // i
				
				IL_0020: ldloc.1      // i
				IL_0021: ldc.i4       10000000 // 0x00989680
				IL_0026: clt
				IL_0028: stloc.3      // V_3

				IL_0029: ldloc.3      // V_3
				IL_002a: brtrue.s IL_0012
			*/
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		static long ArrayForeachMeasure()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (int i in _array)
			{
				int result = i;
			}
			/* the loop compiles to:
				IL_0019: ldloc.1      // V_1
				IL_001a: ldloc.2      // V_2
				IL_001b: ldelem.i4
				IL_001c: stloc.3      // i
				
				IL_001d: nop
				
				IL_001e: ldloc.3      // i
				IL_001f: stloc.s result
				
				IL_0021: nop

				IL_0022: ldloc.2      // V_2
				IL_0023: ldc.i4.1
				IL_0024: add
				IL_0025: stloc.2      // V_2
				
				IL_0026: ldloc.2      // V_2
				IL_0027: ldloc.1      // V_1
				IL_0028: ldlen
				IL_0029: conv.i4
				IL_002a: blt.s IL_0019
			*/
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

	    public static void ForVersusForeach()
	    {
			/* For vs Foreach
				For
				- Pro: fastest but requires an indexer
				- Con: indexer needs all values loaded in memory

				Foreach
				- Pro: works on any collection
				- Pro: loads values on demand (the entire collection does not need to be in the memory)
				- Con: slower because it requires an enumerator (more complex approach)
			*/

		    // prepare lists
		    PrepareListV3();

		    // measurement run
		    long durationA1 = ArrayListForMeasure();
		    long durationA2 = ArrayListForeachMeasure();
		    long durationB1 = GenericListForMeasure();
		    long durationB2 = GenericListForeachMeasure();
		    long durationC1 = ArrayForMeasure();
		    long durationC2 = ArrayForeachMeasure();

		    Console.WriteLine($"ArrayList for: {durationA1}");
		    Console.WriteLine($"ArrayList foreach: {durationA2}");
		    Console.WriteLine($"List<int> for: {durationB1}");
		    Console.WriteLine($"List<int> foreach: {durationB2}");
		    Console.WriteLine($"int[] for: {durationC1}");
		    Console.WriteLine($"int[] foreach: {durationC2}");

			// for native arrays gain is barely visible
			// for others the for loop is up to two times faster

			// the reason is that foreach for arrays compiles to something very similar to for
			// so the compilers avoid all the enumeration's burden
			
			// Non-generic enumerators return the current value as an object. Do not use them for value types to avoid boxing and unboxing
			// Always use generic enumerators if possible

			/* Summary:
				- Array: do not refactor code, not worth it
				- List<> and ArrayList<>: refactor foreach to for to gain considerable improvement
				- ArrayList<>: consider using List<> if possible
				- For value type collections, use IEnumerable<T> instead of IEnumerable to avoid boxing and unboxing
					IEnumerable returns each value as an object
			*/
	    }
    }
}