using Autofac;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;
using System;
using System.Collections.Generic;

namespace DependencyInjectionWithAutofac
{
	// CAUTION:
	// certain types like Owner<T>, IIndex<T> or Meta<T> binds us to Autofac 
	// meaning Autofac objects will start leaking into our domain model
	// and it may be later on difficult to abandon Autofac

	// And another remark - Relationship types are composible!
	// meaning, for example, that the IEnumerable<Func<Owned<ILog>>> is legal
	// which translates to: all implementations, of factiories, lifetime-controlled, of ILog object
	// Wonderful!

    class ImplicitRelationshipTypes
    {
		class Reporting
		{
			// Lazy wrapper around the object
			public Lazy<ConsoleLog> log;

			public Reporting(Lazy<ConsoleLog> log)
			{
				this.log = log ?? throw new ArgumentNullException(paramName: nameof(log));
				Console.WriteLine("Reporting object construction started");
			}

			public void Report()
			{
				// because this is a Lazy type we have to first access Value field
				// and the moment we call Value is when the object is created
				log.Value.Write("Log started");
			}
		}

		public static void DelayedInstatiation()
		{
			// this allows us to make a lazy dependency
			// used for an infrequently used or expensive-to-construct object
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>();
			builder.RegisterType<Reporting>();

			var rep = builder.Build().Resolve<Reporting>();
			rep.Report();
		}

		class ReportingOnce
		{
			// Owned wrapper tells Autofac to not try to dispose this object
			public Owned<ConsoleLog> log;

			public ReportingOnce(Owned<ConsoleLog> log)
			{
				this.log = log ?? throw new ArgumentNullException(paramName: nameof(log));
				Console.WriteLine("ReportingOnce object construction started");
			}

			public void ReportOnce()
			{
				log.Value.Write("Log started");

				// we do not call log.Value.Dispose() - slight difference in syntax
				log.Dispose();
				Console.WriteLine("Log disposed");
			}
		}

		public static void ControlledInstatiation()
		{
			// an owned dependency
			// can be released by the owner when no longer required
			// particulary useful for IDisposable
			// typically, Autofac handles disposal
			// but if we use Owner type we can call Dispose whenever we want
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>();
			builder.RegisterType<ReportingOnce>();


			var repOnce = builder.Build().Resolve<ReportingOnce>();
			Console.WriteLine("repOnce.log.Value == null => " + (repOnce.log.Value == null).ToString());
			repOnce.ReportOnce();

			GC.Collect();

			Console.WriteLine("repOnce.log.Value == null => " + (repOnce.log.Value == null).ToString());
		}

		class ReportingDynamic
		{
			// func means we will get a new instance per request
			public Func<ConsoleLog> log;

			public ReportingDynamic(Func<ConsoleLog> log)
			{
				this.log = log ?? throw new ArgumentNullException(paramName: nameof(log));
			}

			public void ReportDynamic()
			{
				log().Write("Log started");
				log().Write("Log started");
			}
		}

		public static void DynamicInstatiation()
		{
			// the thing that get injected is an auto-generated factory 
			// allows us to Resolve<T>() without typing ourselves to Autofac
			// inject and store a component as Func<T>
			// call myField() to construct the dependency via the container
			// the benefit allows us to drop Autofac in future and the code will stil compile
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>();
			builder.RegisterType<ReportingDynamic>();

			var rep = builder.Build().Resolve<ReportingDynamic>();
			rep.ReportDynamic();
		}

		class ReportingDynamicSMS
		{
			// func means we will get a new instance per request
			public Func<ConsoleLog> log;
			public Func<string, SMSLog> smsLog; // string as parameter and SMSLog as the return type

			public ReportingDynamicSMS(Func<ConsoleLog> log, Func<string, SMSLog> smsLog)
			{
				this.log = log ?? throw new ArgumentNullException(paramName: nameof(log));
				this.smsLog = smsLog ?? throw new ArgumentNullException(paramName: nameof(smsLog));
			}

			public void ReportDynamicSMS()
			{
				log().Write("Log started");
				log().Write("Log started");

				smsLog("+123456").Write("Texting Admins...");
			}
		}

		public static void ParametrizedInstatiation()
		{
			// also makes an auto-generated factory
			// allows us to provide parameters
			// the factory has named parameters so we cannot have diplicate types in parameter list
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>();
			builder.RegisterType<SMSLog>();
			builder.RegisterType<ReportingDynamicSMS>();

			var rep = builder.Build().Resolve<ReportingDynamicSMS>();
			rep.ReportDynamicSMS();
		}

		class ReportingEnumeration
		{
			IList<ILog> logs;

			public ReportingEnumeration(IList<ILog> logs)
			{
				this.logs = logs;
			}

			public void ReportEnumeration()
			{
				foreach(var log in logs)
					log.Write($"Hello, this is {log.GetType().Name}");
			}
		}

		public static void Enumeration()
		{
			// injecting an enumeration get us one of each registred object of type T
			// thanks to this we can simply use all of types with a foreach statement
			// also it allows for safe resolution of objects which may not have been registered (returns an empty list in case of error)
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>().As<ILog>();
			builder.Register(c => new SMSLog("+123456")).As<ILog>();
			builder.RegisterType<ReportingEnumeration>();

			var rep = builder.Build();
			rep.Resolve<ReportingEnumeration>().ReportEnumeration();
		}

		class ReportingMetadata
		{
			Meta<ConsoleLog> log;

			// in case of strongly typed metadata use this
			// private Meta<ConsoleLog, MySettingsObject> log; 

			public ReportingMetadata(Meta<ConsoleLog> log)
			{
				this.log = log ?? throw new ArgumentNullException(paramName: nameof(log));
			}

			public void ReportMetadata()
			{
				log.Value.Write("Log started");

				// weakly typed key is a string and argument is an object that must be casted
				if (log.Metadata["mode"] as string == "verbose")
					log.Value.Write($"VERBOSE MODE: Logger started on {DateTime.Now}");
			}
		}

		public static void MetadataInterrogation()
		{
			// allow us to attach metadata to components
			// lets us make decision when resolving
			// metadata can be weakly type (a dictionary) or strongly typed (lambda syntax)
			// myField.Value to get the actual object
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>().WithMetadata("mode", "verbose");
			
			// in case of strongly typed metadata use this
			//builder.RegisterType<ConsoleLog>()
			//	.WithMetadata<MySettingsObject>(c => c.For(x => x.myField, "myValue"));

			builder.RegisterType<ReportingMetadata>();

			var rep = builder.Build();
			rep.Resolve<ReportingMetadata>().ReportMetadata();
		}

		class ReportingKeyed
		{
			IIndex<string, ILog> logs;

			public ReportingKeyed(IIndex<string, ILog> logs)
			{
				this.logs = logs ?? throw new ArgumentNullException(paramName: nameof(logs));
			}

			public void ReportKeyed() => logs["sms"].Write("Log started");
		}

		public static void KeyedServiceLookup()
		{
			// similat to Enumeration but a dictionary is being used instead of a list
			// instead of As<ILog>() we use Keyed<ILog>("sms")
			var builder = new ContainerBuilder();
			builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
			builder.Register(c => new SMSLog("+123456")).Keyed<ILog>("sms");
			builder.RegisterType<ReportingKeyed>();

			var rep = builder.Build();
			rep.Resolve<ReportingKeyed>().ReportKeyed();
		}
	}
}
