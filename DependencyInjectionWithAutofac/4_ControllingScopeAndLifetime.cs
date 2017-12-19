using Autofac;
using System;
using System.Collections.Generic;

namespace DependencyInjectionWithAutofac
{
    class ControllingScopeAndLifetime
    {
		public static void InstanceScope()
		{
			// scope is essentialy a way to define a lifetime of a component
			// without specifying lifetime, components live as long as the container
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>()
				//.InstancePerDependency(); // default lifetime
				//.SingleInstance(); // with this we only gets one instance no matter how many time it was resolved
				//.InstancePerLifetimeScope(); // we get an instance for every single lifetime we have (variation of the above)
				.InstancePerMatchingLifetimeScope("nameOfTheScope"); // we can give a scope a tag
				// Caution - in Autofac there is no instance for thread - to walk around it we have to create an scope per thread

			var container = builder.Build();

			using (var externalScope = container.BeginLifetimeScope("nameOfTheScope"))
			{
				for (int i = 0; i < 3; i++)
					externalScope.Resolve<ConsoleLog>();

				// we can obviously also nest scopes
				using (var internalScope = externalScope.BeginLifetimeScope())
				{
					for (int i = 0; i < 3; i++)
						internalScope.Resolve<ConsoleLog>();
				}
			}

			try
			{
				using (var anotherScope = container.BeginLifetimeScope())
				{
					anotherScope.Resolve<ConsoleLog>();
				}
			}
			catch (Exception)
			{
				Console.WriteLine("We registred ConsoleLog only in the scope named 'nameOfTheScope'" 
					+ Environment.NewLine 
					+ "so the container is not capable of creating the Log in the 'anotherScope'.");
			}
		}

		// here is the scenario we have one resource which is singleton and another one which is not
		public interface IResource { }

		class SingletonResource : IResource { }

		public class InstancePerDependencyResource : IResource, IDisposable
		{
			public InstancePerDependencyResource() => Console.WriteLine("Instance per dep created");
			public void Dispose() => Console.WriteLine("Instance per dep destroyed");
		}

		// and here is an object that contains both
		public class ResourceManager
		{
			public ResourceManager(IEnumerable<IResource> resources) 
				=>	Resources = resources ?? throw new ArgumentNullException(paramName: nameof(resources));

			public IEnumerable<IResource> Resources { get; set; }
		}

		public static void CaptiveDependencies()
		{
			// long live component hold to short live component
			// short live component will be preserved throughout the whole long-live's lifetime
			var builder = new ContainerBuilder();
			builder.RegisterType<ResourceManager>().SingleInstance();
			builder.RegisterType<SingletonResource>()
			  .As<IResource>().SingleInstance();
			builder.RegisterType<InstancePerDependencyResource>()
			  .As<IResource>();

			using (var container = builder.Build())
			using (var scope = container.BeginLifetimeScope())
			{
				scope.Resolve<ResourceManager>();
			}
		}

		public static void Disposal()
		{
			// basically speaking Autofac controls disposing for us.
			// but we can add ExternallyOwned and then it will not bother to dispose
			var builder = new ContainerBuilder();
			//builder.RegisterType<ConsoleLog>();
			builder.RegisterInstance(new ConsoleLog());
			using (var container = builder.Build())
			{
				using (var scope = container.BeginLifetimeScope())
				{
					scope.Resolve<ConsoleLog>();
				}
			}
		}

		public static void LifetimeEvents()
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<Parent>();

			// adding additional functionality to different stages of the lifetime
			builder.RegisterType<Child>()
				.OnActivating(a =>
				{
					Console.WriteLine("Child activating");
					//a.ReplaceInstance(new OtherClass()); // we can replace this class another here
					// replacing component with another component maybe usefull for example for testing
				})
				.OnActivated(a => {	Console.WriteLine("Child activated"); })
				.OnRelease(a => { Console.WriteLine("Child about to be removed"); });

			// this will actually end up with an exception
			// Autofac does not allows us (yet) to replace something that is registered as an interface
			//builder.RegisterType<ConsoleLog>()
			//	.As<ILog>()
			//	.OnActivating(a =>
			//	{
			//		a.ReplaceInstance(new SMSLog("+123456"));
			//	});

			// but this on the other hand will work
			builder.RegisterType<ConsoleLog>().AsSelf();
			builder.Register<ILog>(c => c.Resolve<ConsoleLog>())
				.OnActivating(a => a.ReplaceInstance(new SMSLog("+123456")));

			using (var scope = builder.Build().BeginLifetimeScope())
			{
				var child = scope.Resolve<Child>();
				var parent = child.Parent;
				Console.WriteLine(child);

				var log = scope.Resolve<ILog>();
				log.Write("Testing");
			}
		}

		public class MyClass : IStartable
		{
			public MyClass() => Console.WriteLine("MyClass ctor");
			public void Start() => Console.WriteLine("Container being built");
		}

		public static void RunningCodeAtStartup()
		{
			// basically if somerhing implements IStartable 
			// whenever we build a container the Start method is called afterwards
			var builder = new ContainerBuilder();
			builder.RegisterType<MyClass>()
			  .AsSelf()
			  .As<IStartable>()
			  .SingleInstance();
			var container = builder.Build();
			container.Resolve<MyClass>();
		}
	}
}
