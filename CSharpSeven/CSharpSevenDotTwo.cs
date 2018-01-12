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
	}
}
