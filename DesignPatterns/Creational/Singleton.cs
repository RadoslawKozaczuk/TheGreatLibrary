using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DesignPatterns.Creational
{
	/*
		Motivation:
		For some components it only makes sense to have one in the system (e.g. repository, factory).
		Sometimes the constructor call is expensive.
		We want to prevent anyone creating additional copies.
		Need to take care of lazy instantiation and thread safety.

		Definition:
		A component which is instantiated only once.
	*/
    class Singleton
    {
		interface IDatabase
		{
			int GetPopulation(string name);
		}

		class SingletonDatabase : IDatabase
		{
			readonly Dictionary<string, int> _capitals;

			private SingletonDatabase()
			{
				_capitals = new Dictionary<string, int>
				{
					["Warsaw"] = 1_735_000,
					["Berlin"] = 3_460_000,
					["Paris"] = 10_067_000
				};
			}

			public int GetPopulation(string name) => _capitals[name];

			// Lazy/Thread Safe approach
			public static IDatabase LazyInstance => _lazyInstance.Value;
			static readonly Lazy<SingletonDatabase> _lazyInstance = new Lazy<SingletonDatabase>(() => new SingletonDatabase());
			
			// Regular approach (although normally in both cases we would simply call it Instance)
			public static IDatabase StdInstance => _stdInstance;
			static readonly SingletonDatabase _stdInstance = new SingletonDatabase();
		}

		// the problem with approach like this is that when we put Singleton everywhere 
		// we start to depend on it.
	    class SingletonRecordFinder
	    {
		    public int GetTotalPopulation(IEnumerable<string> names) => names.Sum(name => SingletonDatabase.StdInstance.GetPopulation(name));
	    }

		// possible solution to that is for example using an interface and simply injecting a database
	    class ConfigurableRecordFinder
	    {
		    readonly IDatabase _database;

		    public ConfigurableRecordFinder(IDatabase database)
		    {
			    _database = database;
		    }

			public int GetTotalPopulation(IEnumerable<string> names) => names.Sum(name => _database.GetPopulation(name));
	    }

	    public class DummyDatabase : IDatabase
	    {
		    public int GetPopulation(string name)
		    {
			    return new Dictionary<string, int>
			    {
					["Warsaw"] = 1_735_000,
				    ["Berlin"] = 3_460_000,
				    ["Paris"] = 10_067_000
				}[name];
		    }
	    }

	    // pretty much reverse approach to the same problem
	    // typically Singleton approach was to prevent people from calling constructors
	    // this case we are doing the opposite you can call as many constructors as you want
	    // but whenever you call a property of the object you access the same data
	    class Monostate
	    {
		    static string _name;

		    public string Name
		    {
			    get => _name;
			    set => _name = value;
		    }
	    }

		public static void Demo()
		{
			var db = SingletonDatabase.StdInstance;
			const string warsaw = "Warsaw";
			WriteLine($"{warsaw} has population {db.GetPopulation(warsaw)}");

			var srf = new SingletonRecordFinder();
			const string berlin = "Berlin";
			WriteLine($"{berlin} has population {srf.GetTotalPopulation(new List<string> { berlin })}");

			var crf = new ConfigurableRecordFinder(new DummyDatabase());
			const string paris = "Paris";
			WriteLine($"{paris} has population {crf.GetTotalPopulation(new List<string> { paris })}");

			// we cannot use Static because static does not have any constructors
			// there is also a variation of Singleton named Monostate
			// the idea is to have a static field but expose it as a non-static
			var mono1 = new Monostate();
			mono1.Name = "alpha";
			WriteLine(Environment.NewLine + $"first instance name: {mono1.Name}");

			var mono2 = new Monostate();
			mono2.Name = "beta";
			WriteLine($"second instance name: {mono2.Name}");
			WriteLine($"first instance name: {mono1.Name}");
		}
	}
}
