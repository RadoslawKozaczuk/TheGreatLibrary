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

		static void NonTrailingNamedArguments()
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

		// until C# 7.2 whenever we passed a structure it was a complete copy
		struct Point
		{
			public double X, Y;

			public Point(double x, double y)
			{
				X = x;
				Y = y;
			}

			public void Reset() => X = Y = 0;

			// we don't want to recreate origin as new Point(), so...
			private static Point origin = new Point();

			// we want to prevent copying of this point
			public static ref readonly Point Origin => ref origin;

			public override string ToString() => $"({X},{Y})";
		}
		
		void ChangeMe(ref Point p)
		{
			p.X++;
		}

		// structs are passed by reference (64 bits n 64 bit systems)
		// 'in' is effectively by-ref and read-only
		// 'in' says that this struct will be past as a reference
		static double MeasureDistance(in Point p1, in Point p2)
		{
			// cannot assign to in parameter
			// p1 = new Point();

			// cannot pass as ref or out method
			// obvious
			// changeMe(ref p2);

			p2.Reset(); // instance operations happen on a copy! It is a protection from walking around the readonly constraint

			var dx = p1.X - p2.X;
			var dy = p1.Y - p2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		// cannot create overloads that differ only in presence?
		// yeah you can, but
		//double MeasureDistance(Point p1, Point p2)
		//{
		//  return 0.0;
		//}

		public static void InParametersAndRefReadonly()
		{
			var p1 = new Point(1, 1);
			var p2 = new Point(4, 5);

			var distance = MeasureDistance(p1, p2);
			WriteLine($"Distance between {p1} and {p2} is {distance}");
			
			var distFromOrigin = MeasureDistance(p1, new Point());
			var alsoDistanceFromOrigin = MeasureDistance(p2, Point.Origin);
			
			// make an ordinary by-value copy
			var copyOfOrigin = Point.Origin;

			// not possible because ref Point.Origin is readonly
			//ref var messWithOrigin = ref Point.Origin;

			ref readonly var originRef = ref Point.Origin;

			// not possible because read only
			//originRef.X = 123;
		}
	}
}
