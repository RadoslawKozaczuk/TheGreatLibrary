using System;
using static System.Console;

namespace CSharpSeven
{
    class CSharpSevenDotTwo
    {
		public static void LeadingDigitSeparators()
		{
			// binary
			var x = 0b_1111_0000;
			WriteLine("0b_1111_0000 = " + x);

			// hex
			var y = 0x_baad_f00d;
			WriteLine("0x_baad_f00d = " + y);

			// and just for reminding - there is not octal numbers is C#
			var stillInt = 0_12344;
			WriteLine("0_12344 = " + stillInt);

			// if for any reason we still want them then
			var z = Convert.ToInt32("1234", 8);
			WriteLine("There is no octal literals in C# -> 1234 = " + z);
		}

		class Base
		{
			private int a; // visible only in this class
			protected internal int b; // derived classes (in ANY assembly) or classes in same assembly
			private protected int c;  // containing class or derived classes in same assembly only 
		}

		class Derived : Base
		{
			public Derived()
			{
				c = 123;
				b = 234; // I can access this because it is in the same assembly
			}
		}
		
		public static void PrivateProtected()
		{
			var d = new Derived();
			d.b = 3;
			//d.c = 5; // no-go
			WriteLine("This example does not provide any output, please check the code.");
		}

		static void DoSomething(int foo, int bar) { }

		public static void NonTrailingNamedArguments()
		{
			// previously we could do something 
			// there was restricted that named parameters has to be after positional ones
			DoSomething(foo: 33, bar: 44);

			// now this restriction is gone
			DoSomething(foo: 33, 44);

			// still illegal - arguments have to be in order
			//doSomething(33, foo:44)

			// the only way to break the order is to name every parameter
			DoSomething(bar: 33, foo: 44);

			WriteLine("This example does not provide any output, please check the code.");
		}
	}
}
