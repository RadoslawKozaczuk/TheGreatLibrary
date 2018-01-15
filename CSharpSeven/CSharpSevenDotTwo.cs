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
	}
}
