using System;
using System.Collections.Generic;
using static System.Console;

namespace DesignPatterns.Creational
{
	/*
		Motivation for using Singleton:
		For some components it only makes sense to have one in the system (e.g. repository, factory).
		Sometimes the constructor call is expensive.
		We want to prevent anyone creating additional copies.
		Need to take care of lazy instantiation and thread safety.

		Definition of Singleton pattern:
		A component which is instantiated only once.
	*/
    class Singleton
    {
		public interface IDatabase
		{
			int GetPopulation(string name);
		}

		public class SingletonDatabase : IDatabase
		{
			private readonly Dictionary<string, int> _capitals;

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
			public static IDatabase LazyInstance => LazyInstanceInternal.Value;
			private static readonly Lazy<SingletonDatabase> LazyInstanceInternal = new Lazy<SingletonDatabase>(() => new SingletonDatabase());

			// Regular approach
			public static IDatabase StdInstance => StdInstanceInternal;
			private static readonly SingletonDatabase StdInstanceInternal = new SingletonDatabase();
		}
		
		public static void Demo()
		{
			var db = SingletonDatabase.StdInstance;
			
			var city = "Warsaw";
			WriteLine($"{city} has population {db.GetPopulation(city)}");
		}
	}
}
