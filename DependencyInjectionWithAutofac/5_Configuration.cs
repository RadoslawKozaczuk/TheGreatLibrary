using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace DependencyInjectionWithAutofac
{	
	// A module is a small class that can bundle a set of related components behind a facade
	// - it simplifies configuration and deployment
	// - exposes a restricted set of configuration parameters that can vary independently of components
	// - modules themselves do not go through dependency injection
	// - there is no reason to call module internall methods
	class Configuration
    {
		interface IVehicle
		{
			void Go();
		}

		class Truck : IVehicle
		{
			private IDriver driver;

			public Truck(IDriver driver)
			{
				this.driver = driver ?? throw new ArgumentNullException(paramName: nameof(driver));
			}

			public void Go() => driver.Drive();
		}

		interface IDriver
		{
			void Drive();
		}

		class CrazyDriver : IDriver
		{
			public void Drive() => Console.WriteLine("Going too fast and crashing into a tree");
		}

		class SaneDriver : IDriver
		{
			public void Drive() => Console.WriteLine("Driving safely to destination");
		}

		class TransportModule : Module
		{
			public bool ObeySpeedLimit { get; set; }

			protected override void Load(ContainerBuilder builder)
			{
				if (ObeySpeedLimit)
					builder.RegisterType<SaneDriver>().As<IDriver>();
				else
					builder.RegisterType<CrazyDriver>().As<IDriver>();

				builder.RegisterType<Truck>().As<IVehicle>();
			}
		}
		public static void UsingModules()
		{
			// essencialy we make a separete component and define it's behavior
			var builder = new ContainerBuilder();
			builder.RegisterModule(new TransportModule { ObeySpeedLimit = true });
			var c = builder.Build();
			c.Resolve<IVehicle>().Go();
		}

		public interface IOperation
		{
			float Calculate(float a, float b);
		}

		public class Addition : IOperation
		{
			public float Calculate(float a, float b) => a + b;
		}

		public class Multiplication : IOperation
		{
			public float Calculate(float a, float b) => a * b;
		}

		public class CalculationModule : Module
		{
			protected override void Load(ContainerBuilder builder)
			{
				builder.RegisterType<Multiplication>().As<IOperation>();
				builder.RegisterType<Addition>().As<IOperation>();
			}
		}

		public static void ConfigurationWithMsConfig()
		{
			// does not work for some reason - exception below
			// System.InvalidOperationException: 'The type 'DependencyInjectionWithAutofac.Addition' could not be found. 
			// It may require assembly qualification, e.g. "MyType, MyAssembly".'

			var configBuilder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("config.json");
			var configuration = configBuilder.Build();

			// configuration module is that part of Autofac than can eat up Microsoft configuration
			var containerBuilder = new ContainerBuilder();
			var configModule = new ConfigurationModule(configuration);
			containerBuilder.RegisterModule(configModule);

			using (var container = containerBuilder.Build())
			{
				float a = 3, b = 4;

				foreach(IOperation op in container.Resolve<IList<IOperation>>())
					Console.WriteLine($"{op.GetType().Name} of {a} and {b} = {op.Calculate(a, b)}");
			}
		}
	}
}
