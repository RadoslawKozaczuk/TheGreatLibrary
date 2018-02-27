using System;

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

	Immutable string
	- Strings is a reference types, and immutable
	- Strings behave as if they are value types. They are assigned and compared by value
	- Strings are thread-safe
*/

namespace PerformanceOptimization
{
    class Program
    {
	    public static void ImmutableStringExample()
	    {
		    string a = "abc";
		    string b = a; // at the moment both references point at the same value
		    b += "d"; // at this moment a secondary string is crated which is a copy of a
				// then the copy is concatenated with "d" string the result is then referenced by the b variable

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

        static void Main()
        {
	        ImmutableStringExample();
			
			Console.WriteLine("Hello World!");
        }
    }
}
