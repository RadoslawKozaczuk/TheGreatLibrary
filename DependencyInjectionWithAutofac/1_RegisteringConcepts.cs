using Autofac;
using System;
using System.Collections.Generic;

namespace DependencyInjectionWithAutofac
{
	class RegisteringConcepts
    {
		public static void WithoutDI()
		{
			var log = new ConsoleLog();
			var engine = new Engine(log);
			var car = new Car(engine, log);
			car.Go();
		}

		public static void Registering_Types()
		{
			// if you forget to register a particular type you will get an exception
			var builder = new ContainerBuilder();

			// the ConsolLog itself is not available because we state that it is going to be resolved only when ILog is requested
			//builder.RegisterType<ConsoleLog>().As<ILog>();
			// so the following statement will result as an error
			//var log = container.Resolve<ConsolLog>();

			// but there is a solution we just have to add AsSelf() at the end
			builder.RegisterType<ConsoleLog>().As<ILog>().AsSelf();

			builder.RegisterType<Engine>();
			builder.RegisterType<Car>();

			IContainer container = builder.Build();

			var log = container.Resolve<ConsoleLog>();

			var car = container.Resolve<Car>();
			car.Go();
		}

		public static void Default_Registration()
		{
			// in case of many registration AutoFac just takes the last one
			var builder = new ContainerBuilder();
			builder.RegisterType<EmailLog>().As<ILog>();

			builder.RegisterType<ConsoleLog>()
				   .As<ILog>()
				   .As<IConsole>() // this fulfiles both of these requests
				   .PreserveExistingDefaults(); // this will not be used as default 

			builder.RegisterType<Engine>();
			builder.RegisterType<Car>();

			IContainer container = builder.Build(); // Email log will be used

			var car = container.Resolve<Car>();
			car.Go();
		}

		public static void ChoicOfConstructor()
		{
			// by default AutoFac chooses most explicit constructor (the highest number of parameters)
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>().As<ILog>();
			builder.RegisterType<Engine>();
			builder.RegisterType<Car>()
			  .UsingConstructor(typeof(Engine)); // but worry not we can specify the constructor
			// but this only afects Car. Engine will use default constructor

			IContainer container = builder.Build();

			var car = container.Resolve<Car>();
			car.Go();
		}

		public static void RegisteringInstances()
		{
			var builder = new ContainerBuilder();

			// if we want the container to use a certain instance of something
			// we use RegisterInstance method and As method which specify
			// to what calls should it respond to
			var log = new ConsoleLog();
			builder.RegisterInstance(log).As<ILog>();

			builder.RegisterType<Engine>();
			builder.RegisterType<Car>()
			  .UsingConstructor(typeof(Engine));

			IContainer container = builder.Build();

			var car = container.Resolve<Car>();
			car.Go();
		}

		public static void LambdaExpressionComponents()
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>().As<ILog>();

			// similar to the example above we but by using lambda expression
			builder.Register((IComponentContext c) =>
			  new Engine(c.Resolve<ILog>(), 123));
			
			builder.RegisterType<Car>();

			IContainer container = builder.Build();

			var car = container.Resolve<Car>();
			car.Go();
		}

		public static void OpenGenericComponents()
		{
			// we can simply register generic components
			var builder = new ContainerBuilder();

			// IList<T> --> List<T>
			// IList<int> --> List<int>
			builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));

			IContainer container = builder.Build();

			var myList = container.Resolve<IList<int>>();
			Console.WriteLine(myList.GetType());
		}
    }
}
