using static System.Console;

namespace DesignPatterns.Structural
{
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
        class SubSystemOne
        {
            public void MethodOne() => WriteLine("\tSubSystemOne Method");
        }

        class SubSystemTwo
        {
            public void MethodTwo() => WriteLine("\tSubSystemTwo Method");
        }

        class SubSystemThree
        {
            public void MethodThree() => WriteLine("\tSubSystemThree Method");
        }

        class SubSystemFour
        {
            public void MethodFour() => WriteLine("\tSubSystemFour Method");
        }

        /// <summary>
        /// Facade is just a class that simplifies or unifies existing APIs.
        /// </summary>
        class FacadeClass
        {
            SubSystemOne _one;
            SubSystemTwo _two;
            SubSystemThree _three;
            SubSystemFour _four;

            public FacadeClass()
            {
                _one = new SubSystemOne();
                _two = new SubSystemTwo();
                _three = new SubSystemThree();
                _four = new SubSystemFour();
            }

            // exposing several components through single interface
            public void MethodA()
            {
                WriteLine("MethodA() -> ");
                _one.MethodOne();
                _two.MethodTwo();
                _four.MethodFour();
                WriteLine("");
            }

            // exposing several components through single interface
            public void MethodB()
            {
                WriteLine("MethodB() -> ");
                _two.MethodTwo();
                _three.MethodThree();
                WriteLine("");
            }
        }

        // Build a Facade to provide a simplified API over a set of classes
        // May wish to (optionally) expose internals through the facade
        // May allow users to 'escalate' to use more complex APIs if they need to
        public static void Demo()
        {
            var facade = new FacadeClass();
            facade.MethodA();
            facade.MethodB();
        }
    }
}
