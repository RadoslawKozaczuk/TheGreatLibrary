using static System.Console;

namespace DesignPatterns.Structural
{
	/*
		Motivation:
		Lets assume we are calling foo.Bar() and that foo is in the same process as Bar().
		What if, later on, you want to put all Foo-related operations into a separate process.
		Proxy provides the same interface but entirely different behavior.
		This is called a communication proxy.

		Definition:
		A class that functions as an interface to a particular resource. That resource may be remote,
		expensive to construct, or may require logging or some other added functionality.
	*/
	class Proxy
    {
		// protection proxy checks whether you have rights to access the particular value
	    public interface ICar
	    {
		    void Drive();
	    }

	    public class Car : ICar
	    {
		    public void Drive() => WriteLine("Car is being driven");
	    }

		// this is our proxy
		// but additionally it takes the driver in constructor
		// and overrides Drive method
	    public class CarProxy : ICar
	    {
		    readonly Car _car = new Car();
		    readonly Driver _driver;

		    public CarProxy(Driver driver)
		    {
			    _driver = driver;
		    }

		    public void Drive()
		    {
			    if (_driver.Age >= 16)
				    _car.Drive();
			    else
				    WriteLine("Driver is too young");
		    }
	    }

	    public class Driver
	    {
		    public int Age { get; set; }

		    public Driver(int age) => Age = age;
	    }

		// we want to implement functionality that car cannot be driven but too young people
		// we make a car proxy which has the same interface but performs additional checks
	    public static void ProtectionProxyDemo()
	    {
			ICar car = new CarProxy(new Driver(12));
		    car.Drive();
		}
	}
}
