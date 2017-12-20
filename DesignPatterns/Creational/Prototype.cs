using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using static System.Console;

namespace DesignPatterns.Creational
{
	static class ExtensionMethods
	{
		// unfortunately, this approach requires all objects to be marked with the Serializable argument
		public static T DeepCopy<T>(this T self)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, self);
				stream.Seek(0, SeekOrigin.Begin);
				var copy = formatter.Deserialize(stream);
				return (T)copy;
			}
		}

		public static object DeepCopyReflection(this object objSource)
		{
			// Get the type of source object and create a new instance of that type
			var sourceType = objSource.GetType();
			var target = Activator.CreateInstance(sourceType);
			// Assign all source property to target object's properties
			foreach (var property in sourceType.GetProperties())
			{
				// Check whether property can be written to
				if (!property.CanWrite) continue;

				// check whether property type is value type, enum or string type
				if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType == typeof(string))
				{
					property.SetValue(target, property.GetValue(objSource, null), null);
				}
				else if(property.PropertyType == typeof(string[]))
				{
					property.SetValue(target, ((string[])property.GetValue(objSource, null)).Clone(), null);
				}
				else
				{
					// property type is object/complex types, so need to recursively call this method until the end of the tree is reached
					var propertyValue = property.GetValue(objSource, null);
					property.SetValue(target, propertyValue?.DeepCopyReflection(), null);
				}
			}
			return target;
		}
	}

	/*
		In real life complicated objects (like cars) aren't designed from scratch, they are modifications of existing designs.
		An existing (partially or fully constructed) design is a Prototype.
		We make a copy (clone) of the prototype and customize it (which requires 'deep copy' support).
		We make the cloning convenient (e.g. via a Factory).

		Definition of the Prototype pattern:
		A partially or fully initialized object that you copy (clone) and make use of it.

		So basically to implement a prototype, partially construct and object and store it somewhere.
		Then clone the prototype (and the question is how to make a deep copy) and customize the resulting instance.
	*/
	class Prototype
    {
	    interface IPrototype<out T>
	    {
		    T DeepCopy();
		}

	    [Serializable] // necessary for binary formatter approach
		public class Address : ICloneable, IPrototype<Address>
	    {
		    public string StreetName { get; set; }
		    public int HouseNumber { get; set; }
			
			// necessary for reflection approach
		    public Address()
		    {
		    }

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

			public override string ToString() => $"{StreetName} {HouseNumber}";

			public object Clone() => new Address(StreetName, HouseNumber);

			public Address DeepCopy() => new Address(StreetName, HouseNumber);
		}

	    [Serializable] // necessary for binary formatter approach
		public class Person : ICloneable, IPrototype<Person>
	    {
		    public string[] Names { get; set; }
		    public Address Address { get; set; }

			// necessary for reflection approach
			public Person()
		    {
		    }

			public Person(string[] names, Address address)
		    {
			    Names = names;
			    Address = address;
		    }

		    public Person(Person other)
		    {
			    Names = (string[])other.Names.Clone();
			    Address = new Address(other.Address);
		    }

			public override string ToString() => $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";

		    public object Clone() => new Person((string[])Names.Clone(), (Address)Address.Clone());

			public Person DeepCopy() => new Person((string[])Names.Clone(), Address.DeepCopy());
		}

		public static void Demo()
		{
			// First approach is to use ICloneable, but the problem with ICloneable interface is that it does not specify whether its implementation
			// should return a shallow copy (object is copied but its object members are just references) or a deep copy (everything is copied)
			// ICloneable interface came out probably before generics so it returns object type which makes its use not very convenient
			// Additionally, if the Array object clone method (new[] {1, 2}.Clone();) returns shallow copy 
			// then it kind of suggest to not use ICloneable interface for deep copy
			var john = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			var jane = (Person)john.Clone();
			jane.Address.HouseNumber = 999;
			jane.Names[0] = "Jane";

			WriteLine("=== ICloneable ===");
			WriteLine(john);
			WriteLine(jane);


			// second approach is to use copy constructors
			john = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			jane = new Person(john);
			jane.Address.HouseNumber = 999;
			jane.Names[0] = "Jane";

			WriteLine(Environment.NewLine + "=== Copy constructor ===");
			WriteLine(john);
			WriteLine(jane);


			// another approach is to use our own interface
			// better idea than ICloneable because we get rid of casting
			// but still in case of having 10 different classes involved we have to implement this 10 times
			john = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			jane = john.DeepCopy();
			jane.Address.HouseNumber = 999;
			jane.Names[0] = "Jane";

			WriteLine(Environment.NewLine + "=== Dedicated interface ===");
			WriteLine(john);
			WriteLine(jane);


			// approach number 4 is to have a binary serializer
			var superJohn = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			var superJane = superJohn.DeepCopy();
			superJane.Address.HouseNumber = 999;
			superJane.Names[0] = "Jane";

			WriteLine(Environment.NewLine + "=== Binary serializer ===");
			WriteLine(superJohn);
			WriteLine(superJane);


			// approach number 5 is by using reflection
			john = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
			jane = (Person)john.DeepCopyReflection();
			jane.Address.HouseNumber = 999;
			jane.Names[0] = "Jane";

			WriteLine(Environment.NewLine + "=== Reflection ===");
			WriteLine(john);
			WriteLine(jane);
		}
	}
}