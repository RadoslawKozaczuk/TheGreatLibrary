using System;

namespace DesignPatterns.Creational
{
	/* Factory and Abstract Factory patterns 
		Object creation logic sometimes becomes too convoluted.
		Name of constructor has to match class name which sometimes makes them not descriptive.
			- name mandated by name of containing type
			- cannot overload with same sets of arguments with different names
			- can turn into 'optional parameter hell'
		Object creation (non-piecewise, unlike Builder in which be build piece by piece) can be outsourced to:
			- a separate function (Factory Method)
			- that may exist in a separate class (Factory)
			- can create hierarchy of factories with Abstract Factory

		The definition of the Factory:
		A component responsible solely for the wholesale (not piecewise) creation of objects.
	*/
	

    class Factory
    {
	    // one of the reason factories exists because constructors aren't that good 
		public class Point
	    {
		    public enum CoordinateSystem
		    {
			    Cartesian,
			    Polar
		    }

			double _x, _y;

			// for example we cannot have two constructors with the same signature
		    protected Point(double x, double y)
		    {
			    _x = x;
			    _y = y;
		    }

			// We have to use universal parameter names then make a description
			// and all of this because we have to stick to the constructor name.
			// This is precisely a problem that factories solve for us.
		    internal Point(double a,
			    double b, // names do not communicate intent
			    CoordinateSystem cs = CoordinateSystem.Cartesian)
		    {
			    switch (cs)
			    {
				    case CoordinateSystem.Polar:
					    _x = a * Math.Cos(b);
					    _y = a * Math.Sin(b);
					    break;
				    default:
					    _x = a;
					    _y = b;
					    break;
			    }
		    }

		    // factory method... 
			// ...which is actually nothing more than a set of static methods that adds some logic to the private constructor
		    public static Point NewCartesianPoint(double x, double y) => new Point(x, y);
		    public static Point NewPolarPoint(double rho, double theta) => new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));

		    public static class Factory
		    {
			    public static Point NewCartesianPoint(double x, double y) => new Point(x, y);
			    public static Point NewPolarPoint(double rho, double theta) => new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
			}
	    }

		// Of course factory methods can be moved to a separate class like so.
		// The problem is we don't want to make Point constructor public but we want to access it somehow.
		// The only way to actually walk it around it is to make it internal member of the Point.
	    class PointFactory
	    {
		    public static Point NewCartesianPoint(float x, float y) => new Point(x, y); // needs to be public
		    public static Point NewPolarPoint(double rho, double theta) => new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));

		}

		public static void FactoryMethodDemo()
		{
			// before factory 
			var p1 = new Point(1, 2, Point.CoordinateSystem.Cartesian);
			var p2 = new Point(1, 2, Point.CoordinateSystem.Polar);

			// internal factory
			var p3 = Point.Factory.NewCartesianPoint(1, 2);
			var p4 = Point.Factory.NewPolarPoint(1, 2);

			// external factory
			var p5 = PointFactory.NewCartesianPoint(1, 2);
			var p6 = PointFactory.NewPolarPoint(1, 2);

			Console.WriteLine("This example does not provide any output, please check the code.");
		}
	}
}
