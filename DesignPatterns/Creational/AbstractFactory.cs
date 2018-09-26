using System;
using System.Collections.Generic;
using static System.Console;

namespace DesignPatterns.Creational
{
	/*
		Motivation:
		AbstractFactory gives away abstract objects in contrary to Factory which gives away concrete objects.
		It simply returns interfaces or abstract classes.
	*/
	class AbstractFactory
	{
		interface IDrink
		{
			void Consume();
		}

		// this class is not going to be given away to anyone
		class Tea : IDrink
		{
			public void Consume() => WriteLine("This tea is nice but I'd prefer it with milk.");
		}

		// same as above
		class Coffee : IDrink
		{
			public void Consume() => WriteLine("This coffee is delicious!");
		}

		// factory has to be able to prepare a drink and return it
		interface IDrinkFactory
		{
			IDrink Prepare(int amount);
		}

		// factories also will not be given away to anyone
		class TeaFactory : IDrinkFactory
		{
			public IDrink Prepare(int amount)
			{
				WriteLine($"Put in tea bag, boil water, pour {amount} ml, add lemon, enjoy!");
				return new Tea();
			}
		}

		class CoffeeFactory : IDrinkFactory
		{
			public IDrink Prepare(int amount)
			{
				WriteLine($"Grind some beans, boil water, pour {amount} ml, add cream and sugar, enjoy!");
				return new Coffee();
			}
		}

		class HotDrinkMachine
		{
            // violates open-closed anytime new drink is added this enum has to be extended
            public enum AvailableDrink { Coffee, Tea }

			readonly Dictionary<AvailableDrink, IDrinkFactory> _factories = 
                new Dictionary<AvailableDrink, IDrinkFactory>();
			
			readonly List<(string name, IDrinkFactory factory)> _namedFactories = 
                new List<(string name, IDrinkFactory factory)>();

			public HotDrinkMachine()
			{
                // first approach - iteration over the enum
                foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
                {
                    var name = Enum.GetName(typeof(AvailableDrink), drink);
                    var factory = (IDrinkFactory)Activator.CreateInstance(
                        Type.GetType($"DesignPatterns.Creational.AbstractFactory+{name}Factory"));

                    // just for the record AsemblyQualifiedName contains weird '+' sign in the middle
                    // we can retrieve it and see on our own eyes by writing the following
                    //var aqn = typeof(CoffeeFactory).AssemblyQualifiedName.ToString();

                    _factories.Add(drink, factory);
                }

                // more sophisticated approach that does not violate open-closed principle
                // we remove enum and use reflection
                foreach (var t in typeof(HotDrinkMachine).Assembly.GetTypes())
                    if (typeof(IDrinkFactory).IsAssignableFrom(t) && !t.IsInterface)
                        _namedFactories.Add((
                            t.Name.Replace("Factory", string.Empty),
                            (IDrinkFactory)Activator.CreateInstance(t)));
            }

			public IDrink MakeDrink()
			{
				WriteLine("Available drinks:");
				for (var index = 0; index < _namedFactories.Count; index++)
				{
                    // deconstructed tuple with the second parameter ignored (syntactic sugar internally this is still there)
					var (name, _) = _namedFactories[index];
					WriteLine($"{index}: {name}");
				}

				while (true)
				{
					var s = ReadLine();
					if (s != null && int.TryParse(s, out var i) && i >= 0 && i < _namedFactories.Count)
					{
						Write("Specify amount: ");
						s = ReadLine();
						if (s != null && int.TryParse(s, out var amount) && amount > 0)
							return _namedFactories[i].factory.Prepare(amount);
					}
					WriteLine("Incorrect input, try again.");
				}
			}

			public IDrink MakeDrink(AvailableDrink drink, int amount) => _factories[drink].Prepare(amount);
		}

		public static void Demo()
		{
			var machine = new HotDrinkMachine();
            var drink = machine.MakeDrink(HotDrinkMachine.AvailableDrink.Tea, 300);
            drink.Consume();

            WriteLine("");
            WriteLine(">You asked for another one sir?");
            WriteLine("");

            var drink2 = machine.MakeDrink();
			drink2.Consume();
		}
	}
}
