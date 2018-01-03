using System;

namespace DesignPatterns
{
	/* === SOLID ===
		1 Single Responsibility Principle
			- A class should only have one reason to change
			- Separation of concerns - different classes handling different, independent task/problems
		2) Open-Close Principle
			- Classes should be open for extension but closed for modification
		3) Liskov Substitution Principle
			- You should be able to substitute a base type for a subtype
		4) Interface Segregation Principle
			- Don't put too much into an interface; split into separate interfaces
			- YAGNI - You Ain't Going to Need It
		5) Dependency Inversion Principle
			- High-level modules should not depend upon low-level ones; use abstractions
	*/
	
	class Program
    {
	    static void Main()
        {
			// Creational Design Patterns
			//Creational.Builder.BuilderDemo();
			//Creational.Builder.FacadedBuilderDemo();
			//Creational.Factory.Demo();
			//Creational.AbstractFactory.Demo();
			//Creational.Prototype.Demo();
			//Creational.Singleton.Demo();

			// Structural Design Patterns
			//Structural.Adapter.Demo();
			//Structural.Bridge.Demo();
			//Structural.Composite.CommonInterfaceDemo();
			//Structural.Composite.BeautifulHierarchyDemo();
			//Structural.Composite.DemoNumberThree();
			//Structural.Decorator.Demo();
			//Structural.Decorator.MultipleInheritanceDemo();
			//Structural.Decorator.DynamicDecoratorsDemo();
			//Structural.Facade.Demo();
			//Structural.Flyweight.Demo();
			//Structural.Proxy.ProtectionProxyDemo();
			//Structural.Proxy.PropertyProxyDemo();

			// Behavioral Design Patterns
			//Behavioral.ChainOfResponsibility.Demo();
			//Behavioral.ChainOfResponsibility.AdvancedDemo();
			//Behavioral.ChainOfResponsibility.DemoNumberThree();
			//Behavioral.Command.Demo();
			//Behavioral.Interpreter.Demo();
			//Behavioral.Iterator.IteratorDemo();
			//Behavioral.Iterator.ArrayBackedPropertiesDemo();
			//Behavioral.Mediator.Demo();
			//Behavioral.Memento.Demo();
			//Behavioral.Memento.AdvancedDemo();
			//Behavioral.NullObject.Demo();
			//Behavioral.Observer.EventDemo();
			//Behavioral.Observer.MemoryLeakDemo();
			//Behavioral.Observer.BindingListDemo();
			//Behavioral.State.Demo();
			//Behavioral.Strategy.DynamicStrategyDemo();
			//Behavioral.TemplateMethod.Demo();
			//Behavioral.Visitor.IntrusiveVisitorDemo();
			Behavioral.Visitor.ReflectiveVisitorDemo();

			// Dimitri's Tips & Tricks
			//LocalInversionOfControl.Demo();

	        Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
	        Console.ReadKey();
		}
    }
}
