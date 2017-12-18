using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace CSharpSevenAndSevenDotOne
{
	class CSharpSevenDotOne
	{
		static string url = "http://google.com/robots.txt";

		// there is no async void, it's Task
		// Task Main if no return 
		// Task<int> Main if you need to return
		static async Task Main(string[] args)
		{
			WriteLine(await new HttpClient().GetStringAsync(url));
		}

		public static void Async_Main() => WriteLine("This example does not provide any output, please check the code.");

		static DateTime GetTimestamps(List<int> items = default) => default;

		// Default literal, one of the slightly meaningless features.
		public static void Default_Expressions()
		{
			// Simplify default expression here
			int a = default;
			WriteLine(a);

			// constants are ok if the inferred type is suitable
			const int c = default;
			WriteLine(c);

			// will not work here
			// const int? d = default; // oops

			// cannot leave defaults on their own
			var numbers = new[] { default, 33, default };
			WriteLine(string.Join(",", numbers));

			// rather silly way of doing things; null is shorter
			string s = default;
			WriteLine(s == null);

			// comparison with default is OK if type can be inferred
			if (s == default)
				WriteLine("Yes, s is default/null");

			// ternary operations
			var x = a > 0 ? default : 1.5;
			WriteLine(x.GetType().Name);
		}

		// typical assembly compilation 
		// csc Program.cs /out:MyDll.dll
		// the new way of compiling assemblies is:
		// csc Program.cs /refout:MyDll.dll
		// it produces an empty API to work with
		public static void Ref_Assemblies() => WriteLine("This example does not provide any output, please check the code.");

		public static void Infer_Tuple_Names()
		{
			// Tuple projection initializers
			// reminder: tuples
			var me = (name: "Dmitri", age: 30);
			WriteLine(me);

			var alsoMe = (me.age, me.name);
			WriteLine(alsoMe.Item1); // before (c# 7)
			WriteLine(alsoMe.name); // new (c# 7.1)

			var result = new[] { "March", "April", "May" } // explicit name not required
				.Select(m => (
				/*Length:*/ m/*?*/.Length, // optionally nullable
				FirstChar: m[0])) // some names (e.g., ToString) disallowed
				.Where(t => t.Length == 5); // note how .Length is available here
			WriteLine(string.Join(",", result));

			// tuples produced by deconstruction
			var now = DateTime.UtcNow;
			var u = (now.Hour, now.Minute);
			var v = (u.Hour, u.Minute) = (11, 12); // this will assign value to u and later u's value to v
			WriteLine(u.Minute);
			WriteLine(v.Minute);
		}

		class Animal
		{
		}

		class Pig : Animal
		{
		}

		// previously these two things were not possible
		class PatternMatchingWithGenerics
		{
			public static void Cook<T>(T animal)
				where T : Animal
			{
				if (animal is Pig pig)
					Write("We cooked and ate the pig...");

				switch (animal)
				{
					case Pig pork:
						WriteLine(" and it tastes delicious!");
						break;
				}
			}	
		}

		static void Pattern_Maching_With_Generics()
		{
			var pig = new Pig();
			PatternMatchingWithGenerics.Cook(pig);
		}
	}
}
