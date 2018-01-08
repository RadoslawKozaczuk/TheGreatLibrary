using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using static System.Console;
using static ReactiveExtensions.Helper;
using System.Reactive.Threading.Tasks;
using Autofac;

namespace ReactiveExtensions
{
    class AdvancedSequenceOperators
    {
		static IObservable<int> SucceedAfter(int attempts)
		{
			int count = 0;
			return Observable.Create<int>(o =>
			{
				WriteLine((count > 0 ? "Ret" : "T") + "rying to do work");
				if (count++ < attempts)
				{
					WriteLine("Failed");
					o.OnError(new Exception());
				}
				else
				{
					WriteLine("Succeeded");
					o.OnNext(count);
					o.OnCompleted();
				}
				return Disposable.Empty;
			});
		}

		public class MyTimer : IDisposable
		{
			Stopwatch stopwatch = new Stopwatch();

			public MyTimer()
			{
				stopwatch.Start();
			}

			public void Dispose()
			{
				stopwatch.Stop();
				WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}msec");
			}
		}

		public static void Exeception_Handling()
		{
			var subj = new Subject<int>();
			var fallback = Observable.Range(1, 3); // this sequence will start after error

			subj
			  // .Catch(Observable.Empty<int>()) // catch-all
			  //.Catch<int, InvalidOperationException>(ex => Observable.Return(-1))
			  .Catch(fallback)
			  .Finally(() => WriteLine("This will be shown regardless")) // finally
			  .Inspect("Catch");

			subj.OnNext(12);
			subj.OnError(new InvalidOperationException());
			subj.OnError(new Exception());
			subj.OnNext(34);
			PressAnyKey();


			Observable.Using( // should be moved
				() => new MyTimer(),
				timer => Observable.Interval(TimeSpan.FromSeconds(1)))
			  .Take(3)
			  .Inspect("Using");

			Thread.Sleep(4000);
			PressAnyKey();


			// OnErrorResumeNext will merge the two sequences
			var seq1 = new Subject<char>();
			var seq2 = new Subject<char>();

			seq1.OnErrorResumeNext(seq2).Inspect("OnErrorResumeNext");

			// merges two sequences whether or not an exception occurs
			seq1.OnNext('a', 'b', 'c')
			  .OnCompleted() // required for merging the two sequences
							 //.OnError<Exception, char>()
			  ;

			seq2.OnNext('d', 'e', 'f');

			// retry re-subscribes to the source and tries again

			// try 4/3, 3/4
			// Retry yields # of attempts required to succeed
			SucceedAfter(3).Retry(5).Inspect("Retry");
		}

		public static void Sequence_Combinators()
		{
			// CombineLatest
			var mechanical = new BehaviorSubject<bool>(true);
			var electrical = new BehaviorSubject<bool>(true);
			var electronic = new BehaviorSubject<bool>(true);

			mechanical.Inspect("mechanical");
			electrical.Inspect("electrical");
			electronic.Inspect("electronic");

			Observable.CombineLatest(mechanical, electrical, electronic)
			  .Select(values => values.All(x => x))
			  .Inspect("Is the system OK?");

			mechanical.OnNext(false);
			PressAnyKey();


			var digits = Observable.Range(0, 10);
			var letters = Observable.Range(0, 10).Select(x => (char)('A' + x));

			letters.Zip(digits, (letter, digit) => $"{letter}{digit}")
			  .Inspect("Zip");

			// And returns Pattern<T1, T2> whose properties are internal
			var punctuation = "!@#$%^&*()".ToCharArray().ToObservable();
			Observable.When( // Plan<> and all that
				digits
				  .And(letters)
				  .And(punctuation)
				  .Then((digit, letter, symbol) => $"{digit}{letter}{symbol}")
			  )
			  .Inspect("And-Then-When");
			PressAnyKey();


			// Concat merges all the sequences into one
			var s1 = Observable.Range(1, 3);
			var s2 = Observable.Range(4, 3);
			s2.Concat(s1).Inspect("Concat");

			// Repeat repeats the sequence as often as is necessary
			s1.Repeat(3).Inspect("Repeat");

			s1.StartWith(2, 1, 0).Inspect("StartWith");
			PressAnyKey();


			// Amb(iguous)
			// will return a value from the sequence that first produces a value
			// will ignore values from all other sequences
			var seq1 = new Subject<int>();
			var seq2 = new Subject<int>();
			var seq3 = new Subject<int>();
			seq1.Amb(seq2).Amb(seq3).Inspect("Amb");
			seq1.OnNext(1); // comment this out
			seq2.OnNext(2);
			seq3.OnNext(3);
			seq1.OnNext(1);
			seq2.OnNext(2);
			seq3.OnNext(3);
			seq1.OnCompleted();
			seq2.OnCompleted();
			seq3.OnCompleted();
			PressAnyKey();


			// Merge pairs up values from multiple sequences
			var foo = new Subject<long>();
			var bar = new Subject<long>();
			var baz = Observable.Interval(TimeSpan.FromMilliseconds(500)).Take(5);

			foo.Merge(bar).Merge(baz).Inspect("Merge");

			foo.OnNext(100);
			Thread.Sleep(1000);
			bar.OnNext(10);
			Thread.Sleep(1000);
			foo.OnNext(1000);
			Thread.Sleep(1000);
		}

		public static void Time_Related_Sequence_Processing()
		{
			// Buffer
			Observable.Range(1, 100)
			  .Buffer(5 /*, 3*/) // elements to skip on subsequent operation
								 // skip value can exceed buffer size, causing
								 // elements to be dropped
			  .Subscribe(x => WriteLine($"Got a group of {x.Count} elements: " + string.Join(",", x)));

			// Delay - simply time-shifts the sequence
			var source = Observable.Interval(TimeSpan.FromSeconds(1))
			  .Take(3);
			var delay = source.Delay(TimeSpan.FromSeconds(2));
			source.Timestamp().Inspect("source");
			delay.Timestamp().Inspect("delay");

			Thread.Sleep(10000);

			// Sample - takes the last value that was available in a given timespan
			var samples = Observable.Interval(TimeSpan.FromSeconds(0.5))
			  .Take(20)
			  .Sample(TimeSpan.FromSeconds(1.75));
			samples.Inspect("Sample");
			samples.ToTask().Wait();

			// Throttle - just like Sample, but the wait window is reset
			// needs to run in Debug mode (external window)
			var subj = new Subject<string>();

			subj // also Timeout, which causes an exception if nothing happens in X seconds
			  .Timeout(TimeSpan.FromSeconds(3), // this argument alone would throw
				Observable.Empty<string>()) // this prevents throwing
											//.Throttle(TimeSpan.FromSeconds(1))
			  .TimeInterval()
			  .Inspect("Throttle");

			string input = string.Empty;
			ConsoleKeyInfo c;
			while ((c = ReadKey()).Key != ConsoleKey.Escape)
			{
				if (char.IsLetterOrDigit(c.KeyChar))
				{
					input += c.KeyChar;
					subj.OnNext(input);
				}
				else if (c.Key == ConsoleKey.Backspace && input.Length > 0)
				{
					input = input.Substring(0, input.Length - 1);
				}
			}
		}

		 class Actor
		{
			protected EventBroker broker;

			public Actor(EventBroker broker)
			{
				this.broker = broker ?? throw new ArgumentNullException(paramName: nameof(broker));
			}
		}

		class FootballCoach : Actor
		{
			public FootballCoach(EventBroker broker) : base(broker)
			{
				broker.OfType<PlayerScoredEvent>()
				  .Subscribe(
					ps =>
					{
						if (ps.GoalsScored < 3)
							WriteLine($"Coach: well done, {ps.Name}!");
					}
				  );

				broker.OfType<PlayerSentOffEvent>()
				  .Subscribe(
					ps =>
					{
						if (ps.Reason == "violence")
							WriteLine($"Coach: How could you, {ps.Name}?");
					});
			}
		}

		class Ref : Actor
		{
			public Ref(EventBroker broker) : base(broker)
			{
				broker.OfType<PlayerEvent>()
				  .Subscribe(e =>
				  {
					  if (e is PlayerScoredEvent scored)
						  WriteLine($"REF: player {scored.Name} has scored his {scored.GoalsScored} goal.");
					  if (e is PlayerSentOffEvent sentOff)
						  WriteLine($"REF: player {sentOff.Name} sent off due to {sentOff.Reason}.");
				  });
			}
		}

		class FootballPlayer : Actor
		{
			private IDisposable sub;
			public string Name { get; set; } = "Unknown Player";
			public int GoalsScored { get; set; } = 0;

			public void Score()
			{
				GoalsScored++;
				broker.Publish(new PlayerScoredEvent { Name = Name, GoalsScored = GoalsScored });
			}

			public void AssaultReferee() => broker.Publish(new PlayerSentOffEvent { Name = Name, Reason = "violence" });

			public FootballPlayer(EventBroker broker, string name) : base(broker)
			{
				Name = name ?? throw new ArgumentNullException(paramName: nameof(name));

				broker.OfType<PlayerScoredEvent>()
				  .Where(ps => !ps.Name.Equals(name))
				  .Subscribe(ps => WriteLine($"{name}: Nicely scored, {ps.Name}! It's your {ps.GoalsScored} goal!"));

				sub = broker.OfType<PlayerSentOffEvent>()
				  .Where(ps => !ps.Name.Equals(name))
				  .Subscribe(ps => WriteLine($"{name}: See you in the lockers, {ps.Name}."));
			}
		}

		class PlayerEvent
		{
			public string Name { get; set; }
		}

		class PlayerScoredEvent : PlayerEvent
		{
			public int GoalsScored { get; set; }
		}

		class PlayerSentOffEvent : PlayerEvent
		{
			public string Reason { get; set; }
		}

		class EventBroker : IObservable<PlayerEvent>
		{
			private readonly Subject<PlayerEvent> subscriptions = new Subject<PlayerEvent>();
			public IDisposable Subscribe(IObserver<PlayerEvent> observer) => subscriptions.Subscribe(observer);

			public void Publish(PlayerEvent pe) => subscriptions.OnNext(pe);
		}
		
		public static void Reactive_Extensions_Event_Broker_With_Autofac()
		{
			var cb = new ContainerBuilder();
			cb.RegisterType<EventBroker>().SingleInstance();
			cb.RegisterType<FootballCoach>();
			cb.RegisterType<Ref>();
			cb.Register((c, p) => new FootballPlayer(c.Resolve<EventBroker>(), p.Named<string>("name")));

			using (var c = cb.Build())
			{
				var referee = c.Resolve<Ref>(); // order matters here!
				var coach = c.Resolve<FootballCoach>();
				var player1 = c.Resolve<FootballPlayer>(new NamedParameter("name", "John"));
				var player2 = c.Resolve<FootballPlayer>(new NamedParameter("name", "Chris"));
				player1.Score();
				player1.Score();
				player1.Score(); // only 2 notifications
				player1.AssaultReferee();
				player2.Score();
			}
		}
	}
}
