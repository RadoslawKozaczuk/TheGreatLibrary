using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		When component A uses component B, it typically assumes that B is non-null.
			You inject B, not B? or some Option<B>?
			You do not check for null on every call.
		There is no option of telling A not to use an instance of B its use its hard coded.
		Thus, we build a no-op, non-functioning inheritor of B and pass it into A.

		Definition:
		A no-op object that conforms to the required interface, satisfying 
		a dependency requirement of some other object and does absolutely nothing at all.
	*/
	class NullObject
    {
	    public interface ILog
	    {
		    void Info(string msg);
		    void Warn(string msg);
	    }

	    class ConsoleLog : ILog
	    {
		    public void Info(string msg) => WriteLine(msg);

		    public void Warn(string msg) => WriteLine("WARNING: " + msg);
	    }

	    public class BankAccount
	    {
		    readonly ILog _log;
		    int _balance;

		    public BankAccount(ILog log)
		    {
			    _log = log;
		    }

		    public void Deposit(int amount)
		    {
			    _balance += amount;
			    // check for null everywhere
			    _log?.Info($"Deposited ${amount}, balance is now {_balance}");
		    }

		    public void Withdraw(int amount)
		    {
			    if (_balance >= amount)
			    {
				    _balance -= amount;
				    _log?.Info($"Withdrew ${amount}, we have ${_balance} left");
			    }
			    else
			    {
				    _log?.Warn($"Could not withdraw ${amount} because balance is only ${_balance}");
			    }
		    }
	    }

		// this does absolutely nothing
	    public sealed class NullLog : ILog
	    {
		    public void Info(string msg) { }
		    public void Warn(string msg) { }
	    }

		// Implement the required interface.
		// Rewrite the methods with empty bodies.
		//	if method is non-void - return default(T).
		//  if this values are ever used - you are in trouble.
		// Supply an instance of NullObject in place of actual object.
		// Dynamic construction is possible although with associated performance implications.
	    // NullObject can be used in dependency injection for example.
		public static void Demo() => WriteLine("This example does not provide any output, please check the code.");
	}
}
