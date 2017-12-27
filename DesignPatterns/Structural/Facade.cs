using static System.Console;

namespace DesignPatterns.Structural
{
	// exposing several components through single interface
	/*
		Motivation:
		Balancing complexity and presentation/usability
		Typical home
			many subsystems(electrical, sanitation etc)
			complex internal structure (e.g. floor layers)
			end user is not exposed to internals
		Same with software
			many systems working to provide flexibility, but
			API consumers want it to 'just work'
			
		Definition:
		Provides a simple, easy to understand/user interface over a large and 
		sophisticated body of code.
	*/
	class Facade
    {
		// Build a Facade to provide a simplified API over a set of classes
		// May wish to(optionally) expose internals through the facade
		// May allow users to 'escalate' to use more complex APIs if they need to
		public static void Demo() => WriteLine("Facade is soooo eazzzyy!");
	}
}
