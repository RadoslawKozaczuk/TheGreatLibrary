using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DesignPatterns
{
	static class ExtensionMethods
	{
		public struct BoolMarker<T>
		{
			public bool Result;
			public T Self;

			// thanks to this we can combine BoolMarkers
			public enum Operation
			{
				None,
				And,
				Or
			};

			internal Operation PendingOp;

			internal BoolMarker(bool result, T self, Operation pendingOp)
			{
				Result = result;
				Self = self;
				PendingOp = pendingOp;
			}

			public BoolMarker(bool result, T self) : this(result, self, Operation.None) { }

			// it returns a copy of the BoolMarker
			public BoolMarker<T> And => new BoolMarker<T>(Result, Self, Operation.And);

			public static implicit operator bool(BoolMarker<T> marker) => marker.Result;
		}

		public static T AddTo<T>(this T self, params ICollection<T>[] colls)
		{
			foreach (var coll in colls)
				coll.Add(self);
			return self;
		}

		public static bool IsOneOf<T>(this T self, params T[] values) => values.Contains(self);

		public static BoolMarker<TSubject> HasNo<TSubject, T>(this TSubject self, Func<TSubject, IEnumerable<T>> props)
			=> new BoolMarker<TSubject>(!props(self).Any(), self);

		public static BoolMarker<TSubject> HasSome<TSubject, T>(this TSubject self, Func<TSubject, IEnumerable<T>> props)
			=> new BoolMarker<TSubject>(props(self).Any(), self);

		public static BoolMarker<T> HasNo<T, U>(this BoolMarker<T> marker, Func<T, IEnumerable<U>> props)
			=> (marker.PendingOp == BoolMarker<T>.Operation.And && !marker.Result)
				? marker // we can simply return the current marker because it already failed (and with the first one equals to false)
				: new BoolMarker<T>(!props(marker.Self).Any(), marker.Self);
	}

	// LocalInversionOfControl is a trick so that instead of calling something on the subject you call it on the object
	class LocalInversionOfControl
	{
		public class Person
		{
			public List<string> Names = new List<string>();
			public List<Person> Children = new List<Person>();
		}

		public static void Demo()
		{
			// Readability example 1
			var list = new List<int>();
			var list2 = new List<int>();

			// it improves readability

			// ENG: "hey list could you please add to yourself the number 24?"
			list.Add(24);

			// ENG: "please add 24 to the list and list2"
			24.AddTo(list, list2);


			// Readability example 2
			var opcode = "AND";

			// look how unreadable is this - goooshh
			if (opcode == "AND" || opcode == "OR" || opcode == "XOR") { }

			// a bit better - if this array contains the opcode then do this
			// but still this is not super readable
			if (new[] { "AND", "OR", "XOR" }.Contains(opcode)) { }

			// another trick that people do. A bit better to read.
			if ("AND OR XOR".Split(' ').Contains(opcode)) { }

			// But here comes the master race!
			if (opcode.IsOneOf("AND", "OR", "XOR")) { }


			// Readability example 3
			// look at this guy he has no names.
			var person = new Person();

			//if (person.Names.Count == 0) {}
			//if (!person.Names.Any())
			if (person.HasSome(p => p.Names).And.HasNo(p => p.Children)) { }

			WriteLine("This example does not provide any output, please check the code.");
		}
	}
}
