using System;
using System.Collections.Generic;
using static System.Console;

namespace DesignPatterns.Creational
{
	/*
		Motivation:
		AbstractFactory gives away abstract objects in contrary to Factory which gives away concrete objects
		it simply returns interfaces or abstract classes.
	*/
	class AbstractFactory
	{
		interface IHotDrink
		{
			void Consume();
		}

		// this class is not going to be given away to anyone
		class Tea : IHotDrink
		{
			public void Consume() => WriteLine("This tea is nice but I'd prefer it with milk.");
		}

		// same as above
		class Coffee : IHotDrink
		{
			public void Consume() => WriteLine("This coffee is delicious!");
		}

		// factory has to be able to prepare a drink and return it
		interface IHotDrinkFactory
		{
			IHotDrink Prepare(int amount);
		}

		// factories also will not be given away to anyone
		class TeaFactory : IHotDrinkFactory
		{
			public IHotDrink Prepare(int amount)
			{
				WriteLine($"Put in tea bag, boil water, pour {amount} ml, add lemon, enjoy!");
				return new Tea();
			}
		}

		class CoffeeFactory : IHotDrinkFactory
		{
			public IHotDrink Prepare(int amount)
			{
				WriteLine($"Grind some beans, boil water, pour {amount} ml, add cream and sugar, enjoy!");
				return new Coffee();
			}
		}

		class HotDrinkMachine
		{
			public enum AvailableDrink // violates open-closed
			{
				Coffee,
				Tea
			}

			readonly Dictionary<AvailableDrink, IHotDrinkFactory> _factories =
				new Dictionary<AvailableDrink, IHotDrinkFactory>();
			
			readonly List<(string name, IHotDrinkFactory factory)> _namedFactories =
				new List<(string name, IHotDrinkFactory factory)>();

			public HotDrinkMachine()
			{
				// something does not work here I give up for now
				// first approach - iteration over the enum
				//foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
				//{
				//	var factory = (IHotDrinkFactory) Activator.CreateInstance(
				//		Type.GetType(
				//			"DesignPatterns.Creational.AbstractFactory." + 
				//			Enum.GetName(typeof(AvailableDrink), drink) + "Factory"));
				//	_factories.Add(drink, factory);
				//}

				// more sophisticated approach that does not violate open-closed principle
				// we remove enum and use reflection
				foreach (var t in typeof(HotDrinkMachine).Assembly.GetTypes())
					if (typeof(IHotDrinkFactory).IsAssignableFrom(t) && !t.IsInterface)
						_namedFactories.Add((
							t.Name.Replace("Factory", string.Empty), 
							(IHotDrinkFactory)Activator.CreateInstance(t)));
			}

			public IHotDrink MakeDrink()
			{
				WriteLine("Available drinks");
				for (var index = 0; index < _namedFactories.Count; index++)
				{
					var tuple = _namedFactories[index];
					WriteLine($"{index}: {tuple.name}");
				}

				while (true)
				{
					var s = ReadLine();
					if (s != null
					    && int.TryParse(s, out var i)
					    && i >= 0
					    && i < _namedFactories.Count)
					{
						Write("Specify amount: ");
						s = ReadLine();
						if (s != null
						    && int.TryParse(s, out var amount)
						    && amount > 0)
						{
							return _namedFactories[i].factory.Prepare(amount);
						}
					}
					WriteLine("Incorrect input, try again.");
				}
			}

			public IHotDrink MakeDrink(AvailableDrink drink, int amount) => _factories[drink].Prepare(amount);
		}

		public static void Demo()
		{
			var machine = new HotDrinkMachine();
			//var drink = machine.MakeDrink(HotDrinkMachine.AvailableDrink.Tea, 300);
			//drink.Consume();

			WriteLine("<You asked for another one.");

			var drink2 = machine.MakeDrink();
			drink2.Consume();
		}
	}
}
