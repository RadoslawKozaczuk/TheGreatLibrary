using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Core;
using System.Collections.Concurrent;
using Autofac.Features.Metadata;

namespace DependencyInjectionWithAutofac
{
    class AdvancedTopics
    {
		public abstract class BaseHandler
		{
			public virtual string Handle(string message) => "Handled: " + message;
		}

		public class HandlerA : BaseHandler
		{
			public override string Handle(string message) => "Handled by A: " + message;
		}

		public class HandlerB : BaseHandler
		{
			public override string Handle(string message) => "Handled by B: " + message;
		}

		public interface IHandlerFactory
		{
			T GetHandler<T>() where T : BaseHandler;
		}

		class HandlerFactory : IHandlerFactory
		{
			public T GetHandler<T>() where T : BaseHandler
			{
				return Activator.CreateInstance<T>();
			}
		}

		public class ConsumerA
		{
			private HandlerA handlerA;

			public ConsumerA(HandlerA handlerA)
			{
				this.handlerA = handlerA ?? throw new ArgumentNullException(paramName: nameof(handlerA));
			}

			public void DoWork() => Console.WriteLine(handlerA.Handle("ConsumerA"));
		}

		public class ConsumerB
		{
			private HandlerB handlerB;


			public ConsumerB(HandlerB handlerB)
			{
				this.handlerB = handlerB ?? throw new ArgumentNullException(paramName: nameof(handlerB));
			}

			public void DoWork() => Console.WriteLine(handlerB.Handle("ConsumerB"));
		}

		public class HandlerRegistrationSource : IRegistrationSource
		{
			public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
			{
				var swt = service as IServiceWithType;
				if (swt == null
					|| swt.ServiceType == null
					|| !swt.ServiceType.IsAssignableTo<BaseHandler>())
				{
					yield break;
				}

				yield return new ComponentRegistration(
					Guid.NewGuid(),
					new DelegateActivator(
						swt.ServiceType,
						(c, p) =>
						{
							var provider = c.Resolve<IHandlerFactory>();
							var method = provider.GetType().GetMethod("GetHandler").MakeGenericMethod(swt.ServiceType);
							return method.Invoke(provider, null);
						}
					),
					new CurrentScopeLifetime(),
					InstanceSharing.None,
					InstanceOwnership.OwnedByLifetimeScope,
					new[] { service },
					new ConcurrentDictionary<string, object>()); // I think concurent dictionary here is not needed - regular would work
			}

			public bool IsAdapterForIndividualComponents => false;
		}
		
		// let us affect the proxess of container type resolution and add additional registrations
		public static void RegistrationSources()
		{
			// this is cimplicated but more or less it works like this
			// we register something that contains something that we didn't register
			// by registering source Autofac will look for these component there
			var b = new ContainerBuilder();
			b.RegisterType<HandlerFactory>().As<IHandlerFactory>();
			b.RegisterSource(new HandlerRegistrationSource());
			b.RegisterType<ConsumerA>();
			b.RegisterType<ConsumerB>();

			var c = b.Build();
			c.Resolve<ConsumerA>().DoWork();
			c.Resolve<ConsumerB>().DoWork();
		}

		public interface ICommand
		{
			void Execute();
		}

		class SaveCommand : ICommand
		{
			public void Execute() => Console.WriteLine("Saving a file");
		}

		class OpenCommand : ICommand
		{
			public void Execute() => Console.WriteLine("Opening a file");
		}

		public class Button
		{
			private ICommand command;
			private string name;

			public Button(ICommand command, string name)
			{
				this.command = command ?? throw new ArgumentNullException(paramName: nameof(command));
				this.name = name;
			}

			public void Click() => command.Execute();

			public void PrintMe() => Console.WriteLine($"I am a button called {name}");
		}

		public class Editor
		{
			private IEnumerable<Button> buttons;

			public Editor(IEnumerable<Button> buttons)
			{
				this.buttons = buttons ?? throw new ArgumentNullException(paramName: nameof(buttons));
			}

			public void ClickAll()
			{
				foreach (var btn in buttons)
				{
					btn.Click();
					btn.PrintMe();
				}
			}
		}

		public static void Adapters()
		{
			// we are getting two buttons, one for each command
			// we rosolve a ICommand -> look into results and for each result create a button
			
			var b = new ContainerBuilder();
			b.RegisterType<SaveCommand>()
				.As<ICommand>()
				.WithMetadata("Name", "Save");
			b.RegisterType<OpenCommand>()
				.As<ICommand>()
				.WithMetadata("Name", "Open");

			// without registering adapter AutoFac would not know that there is a connection
			// between Button and ICommand and we would have eneded up with only one button
			//b.RegisterType<Button>();
			//b.RegisterAdapter<ICommand, Button>(cmd => new Button(cmd, "Turbo"));

			// and very funcy adapter with metadata
			b.RegisterAdapter<Meta<ICommand>, Button>(cmd => new Button(cmd.Value, (string)cmd.Metadata["Name"]));

			b.RegisterType<Editor>();

			var c = b.Build();
			var editor = c.Resolve<Editor>();
			editor.ClickAll();
		}

		public interface IReportingService
		{
			void Report();
		}

		public class ReportingService : IReportingService
		{
			public void Report() =>	Console.WriteLine("Here is your report");
		}

		public class ReportingServiceWithLogging : IReportingService
		{
			private IReportingService decorated;

			public ReportingServiceWithLogging(IReportingService decorated)
			{
				this.decorated = decorated;
			}

			public void Report()
			{
				Console.WriteLine("Commencing log...");
				decorated.Report();
				Console.WriteLine("Ending log...");
			}
		}
		
		public static void Decorators()
		{
			var b = new ContainerBuilder();
			b.RegisterType<ReportingService>().Named<IReportingService>("reporting");
			b.RegisterDecorator<IReportingService>(
			  (context, service) => new ReportingServiceWithLogging(service), "reporting"
			);

			var c = b.Build();
			var r = c.Resolve<IReportingService>();
			r.Report();
		}

		public class ParentWithProp
		{
			public Child Child { get; set; }

			public override string ToString() => "Parent";
		}

		public class Child
		{
			public ParentWithProp Parent { get; set; }

			public override string ToString() => "Child";
		}

		public class ParentWithCtor
		{
			public Child Child;

			public ParentWithCtor(Child child)
			{
				Child = child ?? throw new ArgumentNullException(paramName: nameof(child));
			}

			public override string ToString() => "Parent with a ChildWithProperty";
		}
		
		public static void CircularDependenciesPropToProp()
		{
			// instance per dependency injects new object whenever something is called
			// in case of circular dependency it would explode
			// thats why we use instance per lifetime
			var b = new ContainerBuilder();
			b.RegisterType<ParentWithProp>()
				.InstancePerLifetimeScope()
				.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
			b.RegisterType<Child>()
				.InstancePerLifetimeScope()
				.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

			var c = b.Build();
			Console.WriteLine(c.Resolve<ParentWithProp>().Child);
		}

		public static void CircularDependenciesCtorToProp()
		{
			var b = new ContainerBuilder();
			b.RegisterType<ParentWithCtor>().InstancePerLifetimeScope();
			b.RegisterType<Child>()
				.InstancePerLifetimeScope()
				.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

			var c = b.Build();
			Console.WriteLine(c.Resolve<ParentWithCtor>().Child.Parent);

			// disclaimer: ctor to ctor circular dependancy is not supported
		}
	}
}
