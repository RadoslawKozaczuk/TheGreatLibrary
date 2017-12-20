using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace DesignPatterns.Creational
{
	/*
		In real life complicated objects (like cars) aren't designed from scratch, they are modifications of existing designs.
		An existing (partially or fully constructed) design is a Prototype.
		We make a copy (clone) of the prototype and customize it (which requires 'deep copy' support).
		We make the cloning convenient (e.g. via a Factory).

		Definition of the Prototype pattern:
		A partially or fully initialized object that you copy (clone) and make use of it.
	*/
	class Prototype
    {
		// the problem with ICloneable interface is that it does not specify whether its implementation
		// should be a shallow clone (object is cloned but its object members are copied) or a deep clone (everything is cloned)
		// disclaimer: copy in case of a reference object means copying the reference only
	    public class Address : ICloneable
	    {
		    public readonly string StreetName;
		    public int HouseNumber;

		    public Address(string streetName, int houseNumber)
		    {
			    StreetName = streetName;
			    HouseNumber = houseNumber;
		    }

		    public Address(Address other)
		    {
			    StreetName = other.StreetName;
			    HouseNumber = other.HouseNumber;
		    }

			public override string ToString() => $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";

			// another problem of ICloneable is that it returns an object which is not very convenient
			// IClonable probably came out before generics
		    // also (whining about ICloneable part 3) 
		    // if the Array object clone method returns shallow copy then
		    // it sort of suggest to not use ICloneable interface for deep copy
		    // new[] {1, 2}.Clone(); // returns shallow copy
			public object Clone() => new Address(StreetName, HouseNumber);
	    }

	    public class Person : ICloneable
	    {
		    public readonly string[] Names;
		    public readonly Address Address;

		    public Person(string[] names, Address address)
		    {
			    Names = names;
			    Address = address;
		    }

		    public Person(Person other)
		    {
			    Names = other.Names;
			    Address = new Address(other.Address);
		    }

			public override string ToString() => $"{nameof(Names)}: {string.Join(",", Names)}, {nameof(Address)}: {Address}";

		    public object Clone() => new Person(Names, Address);
	    }
		
		public static void CloningIsBadDemo()
		{
			// first approach is to use IClonable
			var john = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			var jane = (Person)john.Clone();
			jane.Address.HouseNumber = 999;
			jane.Names[0] = "Jane";

			WriteLine(john);
			WriteLine(jane);

			// second approach is to use copy constructors
			john = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			jane = new Person(john);
			jane.Address.HouseNumber = 999; // oops, John is now at 321
			jane.Names[0] = "Jane";

			WriteLine(john);
			WriteLine(jane);
		}
	}
}
