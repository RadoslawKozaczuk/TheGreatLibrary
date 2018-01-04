using System;
using static System.Console;

namespace DesignPatterns
{
	// this monad is kind of a internalized in C# 6.0
	static class MaybeMonad
	{
		public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
			where TResult : class
			where TInput : class
			=> o == null
			? null
			: evaluator(o);

		public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
			where TInput : class // constraint that TInput is a class
		{
			if (o == null) return null;
			return evaluator(o) ? o : null;
		}

		// this simply invokes something
		public static TInput Do<TInput>(this TInput o, Action<TInput> action)
			where TInput : class
		{
			if (o == null) return null;
			action(o);
			return o; // fluent interface - we return ourselves
		}

		// if object is null we return failure otherwise we pass the value
		public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue)
			where TInput : class
			=> o == null
			? failureValue
			: evaluator(o);

		// we can modify it what ever we want - for example we can stop the chain if we get a number -1
		public static TResult WithValue<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
			where TInput : struct
			=> evaluator(o);
	}

	public class Person
	{
		public Address Address { get; set; }
	}

	public class Address
	{
		// we have a nullable thing here
		public string PostCode { get; set; }
	}

	public class MaybeMonadDemo
	{
		public static void Demo(Person p)
		{
			// in C# 6 we can write something like this
			var postcode = p?.Address?.PostCode;

			// before C# 6.0 we had to write all these checks
			if (p != null)
			{
				// unfortunately sometimes we may have some code in between null checks
				if (HasMedicalRecord(p) && p.Address != null)
				{
					CheckAddress(p.Address);
					if (p.Address.PostCode != null)
						postcode = p.Address.PostCode.ToString();
					else
						postcode = "UNKNOWN";
				}
			}

			// the solution is to write extension methods
			// and now we can write chains that not only include logic conditions but also calls
			var postcode2 = p.With(x => x.Address).With(x => x.PostCode);

			var postcode3 = p
				.If(HasMedicalRecord)
				.With(x => x.Address)
				.Do(CheckAddress)
				.Return(x => x.PostCode, "UNKNOWN");

			WriteLine("postcode: " + postcode);
			WriteLine("postcode2: " + postcode2);
			WriteLine("postcode3: " + postcode3);
		}

		static void CheckAddress(Address address) { }

		static bool HasMedicalRecord(Person person) => true;
	}
}
