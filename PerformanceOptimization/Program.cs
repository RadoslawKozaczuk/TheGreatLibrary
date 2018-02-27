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
 */

namespace PerformanceOptimization
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
