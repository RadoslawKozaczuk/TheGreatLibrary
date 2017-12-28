using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Ordinary c# statements are perishable
			Cannot undo a field/property assignment
			Cannot directly serialize a sequence of actions (calls)
		Want an object that represents an operation
			X should change its property Y to Z
			X should do W()
		Uses: GUI commands, multi-level undo/redo, macro recording and more
		
		Definition:
		An object which represents an instruction to perform a particular action. 
		Contains all the information necessary for the action to be taken.
	*/
	class Command
    {
		class BankAccount
		{
			const int OverdraftLimit = -500;
			int _balance;
			
			public void Deposit(int amount)
			{
				_balance += amount;
				WriteLine($"Deposited ${amount}, balance is now {_balance}");
			}

			public bool Withdraw(int amount)
			{
				if (_balance - amount < OverdraftLimit) return false;
				_balance -= amount;
				WriteLine($"Withdrew ${amount}, balance is now {_balance}");
				return true;
			}

			public override string ToString() => $"Balance: {_balance}";
		}

		interface ICommand
		{
			void Call();
			void Undo();
		}

		class BankAccountCommand : ICommand
		{
			public enum Action
			{
				Deposit, Withdraw
			}

			readonly Action _action;
			readonly BankAccount _account;
			readonly int _amount;
			bool _succeeded;

			public BankAccountCommand(BankAccount account, Action action, int amount)
			{
				_account = account ?? throw new ArgumentNullException(nameof(account));
				_action = action;
				_amount = amount;
			}

			public void Call()
			{
				switch (_action)
				{
					case Action.Deposit:
						_account.Deposit(_amount);
						_succeeded = true;
						break;
					case Action.Withdraw:
						_succeeded = _account.Withdraw(_amount);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			public void Undo()
			{
				if (!_succeeded) return;
				switch (_action)
				{
					case Action.Deposit:
						_account.Withdraw(_amount);
						break;
					case Action.Withdraw:
						_account.Deposit(_amount);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		// Command encapsulates all details of an operation in a separate object.
		// Defines instruction for applying the command (either in the command itself, or elsewhere).
		// Optionally defines instructions for undoing the command.
		// Can create composite commands (a.k.a. macros).
		public static void Demo()
		{
			var ba = new BankAccount();
			var commands = new List<BankAccountCommand>
			{
				new BankAccountCommand(ba, BankAccountCommand.Action.Deposit, 1000),
				new BankAccountCommand(ba, BankAccountCommand.Action.Withdraw, 200)
			};

			WriteLine(ba);

			foreach (var c in commands)
				c.Call();

			WriteLine(ba);

			foreach (var c in Enumerable.Reverse(commands))
				c.Undo();

			WriteLine(ba);
		}
	}
}
