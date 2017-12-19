using System;

namespace DesignPatterns
{
	/* === SOLID ===
		1 Single Responsibilit Principle
			- A class should only have one reason to change
			- Separation of concerns - different classes handling different, independent task/problems
		2) Open-Close Principle
			- Classes should be open for extension but closed for modification
		3) Liskov Substitution Principle
			- You should be able to substitute a base type for a subtype
		4) Interface Segregation Principle
			- Don't put too much into an interface; split into separate interafces
			- YAGNI - You Ain't Going to Need It
		5) Dependency Inversion Principle
			- High-level modules should not depend upon low-level ones; use abstractions
	*/
	
	class Program
    {
	    static void Main(string[] args)
        {
			// Creational Design Patterns
	        Creational.Builder.BuilderDemo();
			Creational.Builder.FacadedBuilderDemo();

	        Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
	        Console.ReadKey();
		}
    }
}
