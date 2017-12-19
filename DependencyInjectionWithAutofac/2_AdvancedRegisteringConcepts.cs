using System;
using Autofac;
using Autofac.Core;
using System.Reflection;

namespace DependencyInjectionWithAutofac
{
    class AdvancedRegisteringConcepts
    {
		public static void PassingParameterToRegister()
		{
			var builder = new ContainerBuilder();

			// named parameter
			//builder.RegisterType<SMSLog>()
			//	.As<ILog>()
			//	.WithParameter("phoneNumber", "+12345678");

			// typed parameter
			//builder.RegisterType<SMSLog>()
			//	.As<ILog>()
			//	.WithParameter(new TypedParameter(typeof(string), "+12345678"));

			// resolved parameter
			//builder.RegisterType<SMSLog>()
			//	.As<ILog>()
			//	.WithParameter(new ResolvedParameter(
			//		// predicate
			//		(pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
			//		// value accessor - here we could for example resolve it again from the context
			//		(pi, ctx) => "+12345678")
			//		);

			var random = new Random();
			builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber")))
				.As<ILog>();

			Console.WriteLine("About to build container...");
			var container = builder.Build();

			// postpone the parameter assignment
			var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
			log.Write("MyMessage");
		}

		public static void PropertyAndMethodInjection()
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<Parent>();

			// autowire will try to automatically resolve all internal objects
			//builder.RegisterType<Child>().PropertiesAutowired();

			// simple insert by name
			//builder.RegisterType<Child>()
			//  .WithProperty("Parent", new Parent());

			// insert lamba that resolves Parent object during the Child creation
			builder.Register(c =>
			{
				var child = new Child();
				child.SetParent(c.Resolve<Parent>());
				return child;
			});

			// podpinamy sie pod event onActivated
			builder.RegisterType<Child>()
			  .OnActivated((IActivatedEventArgs<Child> e) =>
			  {
				  var p = e.Context.Resolve<Parent>();
				  e.Instance.SetParent(p);
			  });

			var container = builder.Build();
			var parent = container.Resolve<Child>().Parent;
			Console.WriteLine(parent);
		}

		public static void ScanningForTypes()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var builder = new ContainerBuilder();
			builder.RegisterAssemblyTypes(assembly)
				.Where(t => t.Name.EndsWith("Log")) // we only register the types that ends up with "Log"
				.Except<SMSLog>() // reverse of where, it throws away types we dont need
				// register all types from an assembly in the same way, but one particular type needs to be registred as a ILog and a singleton
				.Except<ConsoleLog>(c => c.As<ILog>().SingleInstance()) 
				.AsSelf(); // addiotionally register it as ConsoleLog

			builder.RegisterAssemblyTypes(assembly)
				.Except<SMSLog>()
				.Where(t => t.Name.EndsWith("Log"))
				.As(t => t.GetInterfaces()[0]);
		}

		public static void ScanningForModules()
		{
			// a module is a self-contained definition of component registrations
			// it extends the Module class

			var builder = new ContainerBuilder();
			builder.RegisterAssemblyModules(typeof(Program).Assembly);
			//builder.RegisterAssemblyModules<ParentChildModule>(typeof(Program).Assembly);

			var container = builder.Build();
			Console.WriteLine(container.Resolve<Child>().Parent);
		}
    }
}
