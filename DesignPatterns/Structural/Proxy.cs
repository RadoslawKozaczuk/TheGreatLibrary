using System;
using System.Collections.Generic;
using static System.Console;

namespace DesignPatterns.Structural
{
	/*
		Motivation:
		Lets assume we are calling foo.Bar() and that foo is in the same process as Bar().
		What if, later on, you want to put all Foo-related operations into a separate process.
		Proxy provides the same interface but entirely different behavior.
		This is called a communication proxy.

		Definition:
		A class that functions as an interface to a particular resource. That resource may be remote,
		expensive to construct, or may require logging or some other added functionality.
	*/
	class Proxy
    {
		#region Protection Proxy
		// protection proxy checks whether you have rights to access the particular value
		interface ICar
	    {
		    void Drive();
	    }

	    class Car : ICar
	    {
		    public void Drive() => WriteLine("Car is being driven");
	    }

		// this is our proxy
		// but additionally it takes the driver in constructor
		// and overrides Drive method
	    class CarProxy : ICar
	    {
		    readonly Car _car = new Car();
		    readonly Driver _driver;

		    public CarProxy(Driver driver)
		    {
			    _driver = driver;
		    }

		    public void Drive()
		    {
			    if (_driver.Age >= 16)
				    _car.Drive();
			    else
				    WriteLine("Driver is too young");
		    }
	    }

	    class Driver
	    {
		    public int Age { get; set; }

		    public Driver(int age) => Age = age;
	    }
		#endregion

		// we want to implement functionality that car cannot be driven but too young people
		// we make a car proxy which has the same interface but performs additional checks
		public static void ProtectionProxyDemo()
	    {
			ICar car = new CarProxy(new Driver(12));
		    car.Drive();
		}

		#region Property Proxy
		public class Property<T> : IEquatable<Property<T>> where T : new()
		{
			T _value;

			// we want to expose this Property
			// the reason why we build this is proxy is that we want to avoid duplicate assignments
			public T Value
			{
				get => _value;
				set
				{
					if (Equals(this._value, value)) return;
					WriteLine($"Assigning value to {value}");
					_value = value;
				}
			}

			public Property() : this(default(T))
			{
			}

			public Property(T value)
			{
				_value = value;
			}

			public static implicit operator T(Property<T> property) => property._value;

			public static implicit operator Property<T>(T value) => new Property<T>(value);

			public bool Equals(Property<T> other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return EqualityComparer<T>.Default.Equals(_value, other._value);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != GetType()) return false;
				return Equals((Property<T>)obj);
			}

			// we are using non readonly value so the hashcode will change
			// what is is null?
			// so this is wrong for this example we do not dive into it
			public override int GetHashCode() => _value.GetHashCode();

			public static bool operator ==(Property<T> left, Property<T> right) => Equals(left, right);

			public static bool operator !=(Property<T> left, Property<T> right) => !Equals(left, right);
		}
		
		class Creature
		{
			readonly Property<int> _agility = new Property<int>();

			public int Agility
			{
				get => _agility.Value;
				set => _agility.Value = value;
			}
		}
		#endregion

		// property proxy is basically the idea of using an object as a property instead of a literal value
		public static void PropertyProxyDemo()
		{
			// thanks to the additional set check we assign the value only once
			var c = new Creature();
			c.Agility = 10;
			c.Agility = 10;
		}
	}
}
