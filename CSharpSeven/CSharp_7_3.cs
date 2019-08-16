using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSharpSeven
{
	unsafe struct FixedStruct
	{
		// allows to pin a variable in memory
		private fixed char _filename[255];
		public fixed int MyField[3];

		// operates on any type that implements a method named DangerousGetPinnableReference()
	}

	struct MyStruct
	{

	}

	public class BaseClass
	{
		public BaseClass(int i, out int j)
		{
			j = i;
		}
	}

	public class DerivedClass : BaseClass
	{
		public DerivedClass(int i)
			: base(i, out var j)
		{
			Console.WriteLine($"j = {j}");
		}
	}

	[Serializable]
	class CSharp_7_3
	{
		// 1. Attributes on Hidden Fields
		[field: NonSerialized]
		public string AutoImplementedProperty { get; set; }

		// previously it was possible but only with event underlying field

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		// 2. In method overload resolution tiebreaker
		// before C# 7.3 these two methods would cause ambiguity
		static void Foo(int arg) => Console.WriteLine("Foo by-value");  // by-value
		static void Foo(in int arg) => Console.WriteLine("Foo by-readonly-reference");
		// now, the by-value overload is preferred to the by-readonly-reference version

		// 3.Extended expression variables in initializers
		// now we can use out variable declarations in field initializers, property initializers
		public int ExpressionVariable => DoSomeThingStupid(out int arg) > 0 ? arg : 0;

		int DoSomeThingStupid(out int arg)
		{
			arg = 1;
			return arg;
		}

		// 4. Additional Generic Constraints
		// now the T can be limited to only unmanaged structures
		List<byte> DoTheList<T>(in T value) where T : unmanaged => null;

		// fixed-sized buffers
		public void Demo()
		{
			unsafe
			{
				var myStruct = new FixedStruct();
				int value = myStruct.MyField[2];
				List<byte> list = DoTheList(myStruct);
			}
		}

		public void Demo2()
		{
			var myStruct = new MyStruct();
			var otherStruct = new MyStruct();

			ref MyStruct refLocal = ref myStruct;
			refLocal = ref otherStruct;

			// stackalloc supports now array initializers
			unsafe
			{
				int* pArr1 = stackalloc int[3] { 1, 2, 3 };

				// or we can omit it
				int* pArr2 = stackalloc int[3] { 1, 2, 3 };
			}

			// equality operators for value tuples
			var tuple1 = ("1", 2);
			var tuple2 = ("2", 3);

			// previously illegal
			var item1Equal = tuple1.Item1 == tuple2.Item1;

			// just for the record equals method for tuples are implemented in a typical manner for value types:
			// So two value tuples are equal if they have the same number of members
			// and each two corresponding members are of the same type
			// and equal according to their default equality comparer.
			// The built-in code for testing a single item for equality between two tuples behaves equivalently to the following implementation:
			var item1EqualImplementation = tuple1.Item1?.GetType() == tuple2.Item1?.GetType()
				&& ((tuple1.Item1 == null && tuple2.Item1 == null) || tuple1.Item1.Equals(tuple2.Item1));
		}
	}
}
