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
		#region "Chain of Responsibility"
		class Creature
		{
			public readonly string Name;
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
			CreatureModifier _next; // this is our chain

			public CreatureModifier(Creature creature)
			{
				Creature = creature ?? throw new ArgumentNullException(nameof(creature));
			}
			
			public void Add(CreatureModifier cm)
			{
				if (_next != null) _next.Add(cm);
				else _next = cm;
			}

			public virtual void Handle() => _next?.Handle();
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
				// This is not the best idea because its permanently modifies the value
				// in case of removing the modifier we would have to recalculate the goblin
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
		#endregion

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

		#region "Chain of Responsibility with a Mediator"
		public class Query
		{
			public string CreatureName;

			public enum Argument
			{
				Attack, Defense
			}

			public Argument WhatToQuery;

			public int Value; // bidirectional

			public Query(string creatureName, Argument whatToQuery, int value)
			{
				CreatureName = creatureName ?? throw new ArgumentNullException(nameof(creatureName));
				WhatToQuery = whatToQuery;
				Value = value;
			}
		}

		public class Game // mediator pattern
		{
			public event EventHandler<Query> Queries; // effectively a chain

			public void PerformQuery(object sender, Query q)
			{
				Queries?.Invoke(sender, q);
			}
		}

		public class CreatureV2
		{
			public string Name;
			readonly Game _game;
			readonly int _attack, _defense;

			public CreatureV2(Game game, string name, int attack, int defense)
			{
				_game = game ?? throw new ArgumentNullException(nameof(game));
				Name = name ?? throw new ArgumentNullException(nameof(name));
				_attack = attack;
				_defense = defense;
			}

			public int Attack
			{
				// getter needs to collect all bonuses - we use Mediator pattern to do that
				get
				{
					var q = new Query(Name, Query.Argument.Attack, _attack);
					_game.PerformQuery(this, q);
					return q.Value;
				}
			}

			// same with defense
			public int Defense
			{
				get
				{
					var q = new Query(Name, Query.Argument.Defense, _defense);
					_game.PerformQuery(this, q);
					return q.Value;
				}
			}

			public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(_attack)}: {Attack}, {nameof(_defense)}: {Defense}";
		}

		// this time we need to have an abstract class because we don't have a linked list anymore
		// IDisposable is implemented to illustrate that we can remove the modifier
		abstract class CreatureModifierV2 : IDisposable
		{
			readonly Game _game;
			protected readonly CreatureV2 Creature;

			protected CreatureModifierV2(Game game, CreatureV2 creature)
			{
				_game = game;
				Creature = creature;
				game.Queries += Handle; // here we subscribe
			}

			// handle does not affect the creature
			// this is abstract so derived classes are expected missing functionality
			protected abstract void Handle(object sender, Query q);

			public void Dispose() => _game.Queries -= Handle;
		}

		class DoubleAttackModifierV2 : CreatureModifierV2
		{
			public DoubleAttackModifierV2(Game game, CreatureV2 creature) : base(game, creature)
			{
			}

			// here we don't change the creature itself
			protected override void Handle(object sender, Query q)
			{
				// we check if the name matches and if the argument is right
				if (q.CreatureName == Creature.Name && q.WhatToQuery == Query.Argument.Attack)
					q.Value *= 2; // here we modify
			}
		}

		class IncreaseDefenseModifierV2 : CreatureModifierV2
		{
			public IncreaseDefenseModifierV2(Game game, CreatureV2 creature) : base(game, creature)
			{
			}

			protected override void Handle(object sender, Query q)
			{
				if (q.CreatureName == Creature.Name && q.WhatToQuery == Query.Argument.Defense)
					q.Value += 2;
			}
		}
		#endregion

		public static void AdvancedDemo()
		{
			var game = new Game();
			var goblin = new CreatureV2(game, "Strong Goblin", 3, 3);
			WriteLine(goblin);

			using (new DoubleAttackModifierV2(game, goblin))
			{
				WriteLine(goblin);
				using (new IncreaseDefenseModifierV2(game, goblin))
				{
					WriteLine(goblin);
				}
			}

			WriteLine(goblin);
		}
	}
}
