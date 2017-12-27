using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace DesignPatterns.Structural
{
	/*
		Motivation:
		Unethical behavior by an employee, who takes the blame? Employee, Manager, CEO?
		Companies have chains of responsibility.

		Definition:
		A chain of components who all get a chance to process a command or a query, optionally
		having default processing implementation and an ability to terminate the processing chain.

		Command Query Separation:
		Command = asking for an action or change (e.g. please set your attack value to 2)
		Query = asking for information (e.g. please give me your attack value)
		CQS = having separate means of sending commands and queries to e.g. direct field access
	*/
	class ChainOfResponsibility
    {
		class Creature
		{
			public string Name;
			public int Attack, Defense;

			public Creature(string name, int attack, int defense)
			{
				Name = name ?? throw new ArgumentNullException(nameof(name));
				Attack = attack;
				Defense = defense;
			}

			public override string ToString() =>  $"{nameof(Name)}: {Name}, {nameof(Attack)}: {Attack}, {nameof(Defense)}: {Defense}";
		}

		// this is our base class - kind of a collection of modifiers
		class CreatureModifier
		{
			// this need to be protected because we intend in to inherit
			protected readonly Creature Creature;
			protected CreatureModifier Next; // this is our chain

			public CreatureModifier(Creature creature)
			{
				Creature = creature ?? throw new ArgumentNullException(nameof(creature));
			}
			
			public void Add(CreatureModifier cm)
			{
				if (Next != null) Next.Add(cm);
				else Next = cm;
			}

			public virtual void Handle() => Next?.Handle();
		}

		class SpellBlockModifier : CreatureModifier
		{
			public SpellBlockModifier(Creature creature) : base(creature)
			{
			}

			// this will effectively block the chain
			public override void Handle() => WriteLine("None shall pass!");
		}

		class DoubleAttackModifier : CreatureModifier
		{
			public DoubleAttackModifier(Creature creature) : base(creature)
			{
			}

			public override void Handle()
			{
				WriteLine($"Doubling {Creature.Name}'s attack");
				Creature.Attack *= 2;
				base.Handle();
			}
		}

		class IncreaseDefenseModifier : CreatureModifier
		{
			public IncreaseDefenseModifier(Creature creature) : base(creature)
			{
			}

			public override void Handle()
			{
				WriteLine("Increasing goblin's defense");
				Creature.Defense += 3;
				base.Handle();
			}
		}

		public static void Demo()
		{
			// new goblin with att 2 and def 2
			var goblin = new Creature("Goblin", 2, 2);
			WriteLine(goblin);

			var root = new CreatureModifier(goblin);
			
			WriteLine("Let's double goblin's attack");
			root.Add(new DoubleAttackModifier(goblin));

			WriteLine("Let's increase goblin's defense");
			root.Add(new IncreaseDefenseModifier(goblin));

			WriteLine("Let's throw a blocking spell on goblin");
			root.Add(new SpellBlockModifier(goblin));

			WriteLine("Let's increase goblin's defense again - this spell should not make any effect");
			root.Add(new IncreaseDefenseModifier(goblin));

			WriteLine(Environment.NewLine + "Execution spell chain");
			root.Handle();
			WriteLine(goblin);
		}
	}
}
