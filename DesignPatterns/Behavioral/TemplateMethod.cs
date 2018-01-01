using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Algorithm can be decomposed into common parts + specifics.
		Strategy pattern does this through composition
			Hugh-level algorithm uses an interface
			Concrete implementations implement the interface
		Template Method does the same thing through inheritance
			Overall algorithm makes use of abstract member
			Inheritors override the abstract members
			Parent template method invoked

		Definition:
		Allows us to define the 'skeleton' of the algorithm, 
		with concrete implementations defined in subclasses.
	*/
	class TemplateMethod
    {
		// let's assume all the games follows the same process
		// we are expected to inherit from this class whenever we want to make a game
		abstract class Game
		{
			public void Run()
			{
				// here we have some abstract invocations.
				// so the algorithm is defined at the high level 
				// but we let the inheritors define the particular parts
				Start();
				while (!HaveWinner)
					TakeTurn();
				WriteLine($"Player {WinningPlayer} wins.");
			}

			protected abstract void Start();
			protected abstract bool HaveWinner { get; }
			protected abstract void TakeTurn();
			protected abstract int WinningPlayer { get; }

			protected int currentPlayer;
			protected readonly int numberOfPlayers;

			public Game(int numberOfPlayers)
			{
				this.numberOfPlayers = numberOfPlayers;
			}
		}

		// simulate a game of chess
		class Chess : Game
		{
			int _maxTurns = 10;
			int _turn = 1;

			public Chess() : base(2)
			{
			}

			protected override void Start() => WriteLine($"Starting a game of chess with {numberOfPlayers} players.");

			protected override bool HaveWinner => _turn == _maxTurns;

			protected override void TakeTurn()
			{
				WriteLine($"Turn {_turn++} taken by player {currentPlayer}.");
				currentPlayer = (currentPlayer + 1) % numberOfPlayers;
			}

			protected override int WinningPlayer => currentPlayer;
		}
		
		public static void Demo()
		{
			var chess = new Chess();
			chess.Run();
		}
	}
}
