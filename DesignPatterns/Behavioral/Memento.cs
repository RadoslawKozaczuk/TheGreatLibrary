using System.Collections.Generic;
using static System.Console;

namespace DesignPatterns.Behavioral
{
    /*
		Motivation:
		An object or system goes through changes
			a bank account gets deposits and withdrawals
		There are different ways of navigating those changes.
		One way is to record every change (Command) and teach a command to 'undo' itself.
		Another is to simply save snapshots of the system at the particular point in time.

		Definition:
		A token/handle representing the system state. Lets us roll back to the state when the token was generated.
		May or may not directly expose state information.
	*/
    class Memento
	{
		#region Simple Memento
		class SimpleMemento
		{
			// we provide an API that gives us a token that allows as to roll the system back to particular state
			public int Balance { get; }

			public SimpleMemento(int balance)
			{
				Balance = balance;
			}
		}

		class BankAccount
		{
			int _balance;

			public BankAccount(int balance)
			{
				_balance = balance;
			}

			// we return Memento which is our token that holds the state that system was in
			public SimpleMemento Deposit(int amount)
			{
				_balance += amount;
				return new SimpleMemento(_balance);
			}

			// account can be restored from a Memento
			public void Restore(SimpleMemento m) => _balance = m.Balance;

			public override string ToString() => $"Balance: {_balance}";
		}
		#endregion

		public static void Demo()
		{
			var ba = new BankAccount(100);
			var m1 = ba.Deposit(50); // 150
			var m2 = ba.Deposit(25); // 175
			WriteLine(ba);

			// restore to m1
			ba.Restore(m1);
			WriteLine(ba);

			ba.Restore(m2);
			WriteLine(ba);
		}

		#region "Memento with Undo/Redo"
		public class MementoV2
		{
			public int Balance { get; }

			public MementoV2(int balance)
			{
				Balance = balance;
			}
		}

		public class BankAccountV2
		{
			readonly List<MementoV2> _changes = new List<MementoV2>();
			int _balance;
			int _current; // which Memento we are currently on

			public BankAccountV2(int balance)
			{
				_balance = balance;
				_changes.Add(new MementoV2(balance)); // first snapshot
			}

			public MementoV2 Deposit(int amount)
			{
				_balance += amount;
				var m = new MementoV2(_balance);
				_changes.Add(m);
				_current++;
				return m;
			}

			public void Restore(MementoV2 m)
			{
				if (m == null) return;

				_balance = m.Balance;
				_changes.Add(m);
				_current = _changes.Count - 1;
			}

			public MementoV2 Undo()
			{
				if (_current <= 0) return null;

				var m = _changes[--_current];
				_balance = m.Balance;
				return m;
			}

			public MementoV2 Redo()
			{
				if (_current + 1 >= _changes.Count) return null;

				var m = _changes[++_current];
				_balance = m.Balance;
				return m;
			}

			public override string ToString() => $"Balance: {_balance}";
		}
		#endregion

		// Mementos are used to roll back states arbitrarily
		// A memento is simply a token/handle class with (typically) no functions of its own
		// A memento is not required to expose directly the state(s) to which it reverts the system
		// Can be used to implement undo/redo
		public static void AdvancedDemo()
		{
			var ba = new BankAccountV2(100);
			ba.Deposit(50);
			ba.Deposit(25);
			WriteLine(ba);

			ba.Undo();
			WriteLine($"Undo 1: {ba}");
			ba.Undo();
			WriteLine($"Undo 2: {ba}");
			ba.Redo();
			WriteLine($"Redo 2: {ba}");
		}
	}
}
