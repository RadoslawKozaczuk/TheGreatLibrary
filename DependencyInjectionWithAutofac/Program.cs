using System;

#region Key Concepts Explaination
/*	=== Dependency Injection Explainayion ===

Our goals:
we want to be able to manage dependencies and easily substitute components
we want system to be extensible and have flexible control over the part the system is built

Explaination of key concepts:
Car car = new Car(new Engine()); - car depends on Engine 

Manually supplying Engine to Car is not great because
- you have to do it correctly in every place where you make a car
- Engine might have its own dependencies and it might be a very deep tree of dependencies 
    and satisfing all these dependencies may be problematic. So it does not scale.

Solution is ofcourse Dependency Injection - all dependencies are magically provided
- no need to use the "new" keyword
- deep dependency trees are resolved

How does it work?
- put all dependencies as constructor arguments (constructor injection)
- configure a container to satisfy dependencies in a particular way
- use the container to initialize a component, including all of its dependecies

Advantages:
- reduction of boilerplate code
- one central place where everything is configured
- can externalize configuration (consumer can change modules like different logging libraries without need to recompile)
- simple lifetime control (you make your component a singleton)
- encapsulation

Disadvantages:
- creates an expectation that configuration is done by constructon code (if someone forget that everything happends when we actually build dependency container)
- behavior separated from construction makes cord hard to read
- requires an upfront development effort (adding container at the end if very time consuming)
- can cause an explosion of types (because esentially DI drives you into the mindset 
    of expressing component as services which the provide sometimes making people create to many types)
*/

/*	=== Inversion of Control exaplaination ===

Ordinary control: myList.Add(42);
Inverted control: 42.AddTo(myList);

Dependency injection is a form of inversion of control.
Instead of using ordinary object composition we delegate that to the IoC Container, telling the container how you want the object to be built.
*/

/* === Autofac terminology ===
	Component - a body of code that declares the services it provides and the dependencies it consumes
	Service - a contract between a providing and consuming component
	Dependency - a service required by a component
	Container - manages the components that make up the application
*/
#endregion

namespace DependencyInjectionWithAutofac
{
    class Program
    {
        static void Main(string[] args)
        {
			// 1. Registering Concepts
			//Registering_Concepts.WithoutDI();
			//Registering_Concepts.Registering_Types();
			//Registering_Concepts.Default_Registration();
			//Registering_Concepts.ChoicOfConstructor();
			//Registering_Concepts.RegisteringInstances();
			//Registering_Concepts.LambdaExpressionComponents();
			//Registering_Concepts.OpenGenericComponents();


			// 2. Advanced Registering Concepts
			//Advanced_Registering_Concepts.PassingParameterToRegister();
			//Advanced_Registering_Concepts.PropertyAndMethodInjection();
			//Advanced_Registering_Concepts.ScanningForTypes();
			//Advanced_Registering_Concepts.ScanningForModules();


			// 3. Implicit Relationship Types
			//Implicit_Relationship_Types.DelayedInstatiation();
			//Implicit_Relationship_Types.ControlledInstatiation();
			//Implicit_Relationship_Types.DynamicInstatiation();
			//Implicit_Relationship_Types.ParametrizedInstatiation();
			//Implicit_Relationship_Types.Enumeration();
			//Implicit_Relationship_Types.MetadataInterrogation();
			//Implicit_Relationship_Types.KeyedServiceLookup();


			// 4. Chapter Controlling Scope And Lifetime
			//Controlling_Scope_And_Lifetime.InstanceScope();
			//Controlling_Scope_And_Lifetime.CaptiveDependencies();
			//Controlling_Scope_And_Lifetime.Disposal();
			//Controlling_Scope_And_Lifetime.LifetimeEvents();
			//Controlling_Scope_And_Lifetime.RunningCodeAtStartup();


			// 5. Configuration
			//Configuration.UsingModules();
			Configuration.ConfigurationWithMsConfig();

			Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
			Console.ReadKey();
		}
    }
}
