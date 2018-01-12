using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace CSharpSeven
{
	class CSharpSeven
	{
		public static void OutVariables()
		{
			DateTime dt;
			if (DateTime.TryParse("01/01/2017", out dt))
			{
				WriteLine($"Old-fashioned parse: {dt}");
			}

			// variable declaration is an expression, not a statement
			if (DateTime.TryParse("02/02/2016", out var dt2))
			{
				WriteLine($"New parse: {dt2}");
			}

			// the scope of dt2 extends outside the if block
			WriteLine($"I can use dt2 here: {dt2}");

			// what if the parse fails?
			int.TryParse("abc", out var i);
			WriteLine($"i = {i}"); // default value
		}

		class Shape
		{
		}

		class Rectangle : Shape
		{
			public int Width = 0, Height = 0;
		}

		class Circle : Shape
		{
			public int Diameter = 0;
		}

		public static void PatternMaching()
		{
			// difference between is and as 
			// IS - Is Operator is used to Check the Compatibility of an Object with a given Type and it returns the result as a Boolean(True Or False).
			// internally IS is an equivalent to:
			// Student s = obj is Student ? (Student)obj : (Student)null;
			// AS - As Operator is used for Casting of Object to a given Type or a Class. 
			var someShape = new Shape();


			// usage of is operator
			if (someShape is Rectangle)
			{
				var rc = (Rectangle)someShape;
			}
			else if (someShape is Circle)
			{
				// ...
			}

			// usage of as operator
			var rect = someShape as Rectangle;
			if (rect != null) // nonnull
			{
				//...
			}

			// new combined operator is with a variable declaration and assignment
			if (someShape is Rectangle r)
			{
				var imUsingIt = r.Height;
			}

			// can also do the inverse
			if (!(someShape is Circle cc))
			{
				// not a circle!
			}

			// same can be used in switch
			switch (someShape)
			{
				case Circle c:
					break;
				// additionally very convenient when keyword
				case Rectangle sq when (sq.Width == sq.Height):
					break;
			}

		}

		class Point
		{
			public int X, Y;

			public void Deconstruct(out string s)
			{
				s = $"{X}-{Y}";
			}

			public void Deconstruct(out int x, out int y)
			{
				x = X;
				y = Y;
			}
		}


		// old way
		static Tuple<double, double> SumAndProduct(double a, double b) => Tuple.Create(a + b, a * b);

		// new way with field names
		// requires System.ValueTuple NuGet package
		// originally with no names
		static (double sum, double product) NewSumAndProduct(double a, double b) => (a + b, a * b);

		public static void Tuples()
		{
			var sp = SumAndProduct(2, 5);
			// sp.Item1 ugly
			WriteLine($"sum = {sp.Item1}, product = {sp.Item2}");

			var sp2 = NewSumAndProduct(2, 5);
			WriteLine($"new sum = {sp2.sum}, product = {sp2.product}");
			WriteLine($"Item1 = {sp2.Item1}");
			WriteLine(sp2.GetType());

			// converting to ValueTuple loses all info
			var vt = sp2;
			// back to Item1, Item2, etc...
			var item1 = vt.Item1; // :(

			// can use var below
			//(double sum, var product) = NewSumAndProduct(3, 5);
			var (sum, product) = NewSumAndProduct(3, 5);

			// note! var works but double doesn't
			// double (s, p) = NewSumAndProduct(3, 4);

			WriteLine($"sum = {sum}, product = {product}");
			WriteLine(sum.GetType());

			// also assignment
			double s, p;
			(s, p) = NewSumAndProduct(1, 10);

			// tuple declarations with names
			//var me = new {name = "Dmitri", age = 123}; // AnonymousType
			var me = (name: "Dmitri", age: 123);
			WriteLine(me);
			WriteLine(me.GetType());

			// names are not part of the type:
			WriteLine("Fields: " + string.Join(",", me.GetType().GetFields().Select(pr => pr.Name)));
			WriteLine("Properties: " + string.Join(",", me.GetType().GetProperties().Select(pr => pr.Name)));

			WriteLine($"My name is {me.name} and I am {me.age} years old");
			// proceed to show return: TupleElementNames in dotPeek (internally, Item1 etc. are used everywhere)

			// unfortunately, tuple names only propagate out of a function if they're in the signature
			var snp = new Func<double, double, (double sum, double product)>((a, b) => (sum: a + b, product: a * b));
			var result = snp(1, 2);
			// there's no result.sum unless you add it to signature
			WriteLine($"sum = {result.sum}");

			// deconstruction - it requires method Deconstruct in the Point object
			var pt = new Point { X = 2, Y = 3 };
			var (x, y) = pt; // interesting error here
			WriteLine($"Got a point x = {x}, y = {y}");

			// can also discard values the second value goes to thrash
			(int z, _) = pt;
		}

		static Tuple<double, double> SolveQuadratic(double a, double b, double c)
		{
			// a local function in a method
			double CalculateDiscriminant() => b * b - 4 * a * c;

			// it can take arguments if we want
			//double CalculateDiscriminant(double aa, double bb, double cc) => bb * bb - 4 * aa * cc;

			// in C#6 we would have to do something like this 
			//var CalculateDiscriminant = new Func<double, double, double, double>((aa, bb, cc) => bb * bb - 4 * aa * cc);

			// or create another method
			//private static double CalculateDiscriminant(double a, double b, double c)
			//{
			//  return b * b - 4 * a * c;
			//}

			var disc = CalculateDiscriminant();
			var rootDisc = Math.Sqrt(disc);
			return Tuple.Create(
			  (-b + rootDisc) / (2 * a),
			  (-b - rootDisc) / (2 * a));
		}

		public static void LocalFunctions()
		{
			var result = SolveQuadratic(1, 10, 16);
			WriteLine(result);
		}

		static ref int Find(int[] numbers, int value)
		{
			for (int i = 0; i < numbers.Length; i++)
			{
				if (numbers[i] == value)
					return ref numbers[i];
			}

			// cannot do by value return
			//return -1;

			// cannot return a reference to a local variable obviously 
			//int fail = -1;
			//return ref fail;

			throw new ArgumentException("error");
		}

		static ref int Min(ref int x, ref int y)
		{
			// ternary operator does not handle ref
			//return x < y ? (ref x) : (ref) y;
			//return ref (x < y ? x : y);
			if (x < y) return ref x;
			return ref y;
		}

		public static void RefReturnsAndLocals()
		{
			// reference to a local element
			int[] numbers = { 1, 2, 3 };
			ref int refToSecond = ref numbers[1];
			var valueOfSecond = refToSecond;

			// cannot rebind
			// refToSecond = ref numbers[0];

			refToSecond = 123;
			WriteLine(string.Join(",", numbers)); // 1, 123, 3

			// reference persists even after the array is resized
			Array.Resize(ref numbers, 1);
			WriteLine($"second = {refToSecond}, array size is {numbers.Length}");
			refToSecond = 321;
			WriteLine($"second = {refToSecond}, array size is {numbers.Length}");
			//numbers.SetValue(321, 1); // will throw

			// won't work with lists
			var numberList = new List<int> { 1, 2, 3 };
			//ref int second = ref numberList[1]; // property or indexer cannot be out


			int[] moreNumbers = { 10, 20, 30 };
			//ref int refToThirty = ref Find(moreNumbers, 30);
			//refToThirty = 1000;

			// disgusting use of language
			Find(moreNumbers, 30) = 1000;

			WriteLine(string.Join(",", moreNumbers));

			// too many references:
			int a = 1, b = 2;
			ref var minRef = ref Min(ref a, ref b);

			// non-ref call just gets the value
			int minValue = Min(ref a, ref b);
			WriteLine($"min is {minValue}");
		}

		public class Person
		{
			private int id;

			private static readonly Dictionary<int, string> names = new Dictionary<int, string>();

			// constructor and destructor as an expression body
			public Person(int id, string name) => names.Add(id, name);
			~Person() => names.Remove(id);

			public string Name
			{
				// getters and setters and be changes into an expression body
				get => names[id];
				set => names[id] = value;
			}
		}

		public static void ExpressionBodiedMembers() => WriteLine("This example does not provide any output, please check the code.");

		class ExceptionThrower
		{
			string Name { get; set; }

			// in c#7 we can put throw in a null check expression
			public ExceptionThrower(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

			// in c#7 we can put throw in a ternary operator
			public int GetValue(int n) => n > 0 ? n + 1 : throw new Exception();
		}

		public static void ThrowExpressions()
		{
			int v = -1;
			try
			{
				var te = new ExceptionThrower("");
				v = te.GetValue(-1); // does not get defaulted!
			}
			catch (Exception e)
			{
				WriteLine(e);
			}
			finally
			{
				WriteLine(v);
			}
		}

		static async Task<long> GetDirSize(string dir)
		{
			if (!Directory.EnumerateFileSystemEntries(dir).Any())
				return 0;

			// Task<long> is return type so it still needs to be instantiated

			return await Task.Run(() => Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
			  .Sum(f => new FileInfo(f).Length));
		}

		// C# 7 lets us define custom return types on async methods
		// main requirement is to implement GetAwaiter() method

		// ValueTask is a good example

		// need NuGet package
		static async ValueTask<long> NewGetDirSize(string dir)
		{
			if (!Directory.EnumerateFileSystemEntries(dir).Any())
				return 0;

			// Task<long> is return type so it still needs to be instantiated

			return await Task.Run(() => Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
			  .Sum(f => new FileInfo(f).Length));
		}

		public static void GeneralizedAsyncReturnTypes()
		{
			// async methods used to require void, Task or Task<T>

			// C# 7 allows other types such as ValueType<T> - prevent
			// allocation of a task when the result is already available
			// at the time of awaiting
			WriteLine(NewGetDirSize(@"c:\temp").Result);
		}

		public static void LiteralImprovments()
		{
			int a = 123_321______123;
			WriteLine(a);

			// cannot do trailing
			//int c = 1_2_3___;

			// also works for hex
			long hex = 0xAB_BC_D1FF;
			WriteLine(hex);

			// also binary
			var bin = 0b1110_0010_0011;
			WriteLine(bin);
		}
	}
}
