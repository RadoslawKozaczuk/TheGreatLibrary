using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static ReactiveExtensions.Helper;
using static System.Console;

namespace ReactiveExtensions
{
	// basically sequences works like LINQ but with the difference that they are also affected by time
	class FundamentalSequenceOperators
	{
		static IObservable<string> Blocking()
		{
			var subj = new ReplaySubject<string>();
			subj.OnNext("foo");
			subj.OnNext("bar");
			subj.OnCompleted();
			Thread.Sleep(3000);
			return subj;
		}

		static IObservable<string> Nonblocking() =>  
			Observable.Create<string>(observer =>
			{
				observer.OnNext("foo");
				observer.OnNext("bar");
				observer.OnCompleted();
				Thread.Sleep(3000);
				return Disposable.Empty;
			});

		static void TimerElapsed(object sender, ElapsedEventArgs e) => WriteLine($"tock {e.SignalTime}");

		static IObservable<T> Return<T>(T value) => 
			Observable.Create<T>(x =>
			{
				x.OnNext(value);
				x.OnCompleted();
				return Disposable.Empty;
			});

		// simple factory methods
		static void SequenceCreation(string[] args)
		{
			// shortcuts to creating ReplaySubject
			var meaningOfLife = Observable.Return<int>(42); // ^^^ try recreating above
			meaningOfLife.Subscribe(WriteLine);

			// is equivalent to
			var mol = new ReplaySubject<int>();
			mol.OnNext(42);
			mol.OnCompleted();

			var empty = Observable.Empty<int>();
			// basically, same as a replay subject that's immediately completed

			var never = Observable.Never<int>();
			// no items, doesn't terminate

			var throws = Observable.Throw<string>(new Exception("oops"));
		}

		public static void Observable_Create()
		{
			// create an observable sequence from a specified subscribe method implementation
			// lets you specify a delegate any time a subscription is made

			//Blocking().Subscribe(s => Console.WriteLine($"Got {s}"));
			//Nonblocking().Subscribe(s => Console.WriteLine($"Got {s}"));

			// 2. Show Return<T>

			// 3. Returning an action instead of an IDisposable
			//      var o = Observable.Create<string>(observer =>
			//      {
			//        var timer = new Timer(1000);
			//        timer.Elapsed += (sender, e) => observer.OnNext($"tick {e.SignalTime}");
			//        timer.Elapsed += TimerElapsed;
			//        timer.Start();
			//        return Disposable.Empty;
			//      });
			//
			//      var sub = o.Subscribe(Console.WriteLine);
			//      Console.ReadLine();
			//
			//      sub.Dispose();
			//      // want to do timer.Dispose here
			//
			//      Console.ReadLine(); // still getting the tocks
			//                          // have not released the 2nd event handler;
			//                          // have not disposed of the timer


			var o = Observable.Create<string>(
			  observer =>
			  {
				  var timer = new Timer(1000);
				  timer.Elapsed += (sender, e) => observer.OnNext($"tick {e.SignalTime}");
				  timer.Elapsed += TimerElapsed;
				  timer.Start();

				  // return a lambda that removes the timer
				  // if we returned Disposable.Empty here "tock" would go on forever even when object o is set to null
				  //return Disposable.Empty;
				  return () =>
				  {
					  timer.Elapsed -= TimerElapsed;
					  timer.Dispose();
				  };
			  });

			var sub = o.Subscribe(WriteLine);
			ReadKey();

			sub.Dispose();
			WriteLine("Subscription disposed");
			// not getting tocks here

			ReadKey();
			o = null;
			WriteLine("Object o has been destroyed");
		}
		
		public static void Sequence_Generators()
		{
			var andWeAreDone = new Action(() => WriteLine("And we are done" + Environment.NewLine));
			//var pressAnyKey = new Action(() =>
			//{
			//	WriteLine("=== Press any key to continue" + Environment.NewLine);
			//	ReadKey();
			//});


			WriteLine("this Observable.Timer waits 1 second and then publishes one value");
			using (Observable.Timer(TimeSpan.FromSeconds(1))
				.Subscribe(WriteLine, andWeAreDone))
			{
				PressAnyKey();
			}



			WriteLine("this Observable.Timer waits 1 second and then produces a value half second");
			using (Observable.Timer(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(1000))
				.Subscribe(WriteLine, andWeAreDone))
			{
				PressAnyKey();
			}


			WriteLine("this Observable.Generate waits 1s then produces a value every 0.5s until the next value is > 100");
			// merge timing functionality and sequence generator functionality
			var dueTime = TimeSpan.FromMilliseconds(1000);
			var period = TimeSpan.FromMilliseconds(500);
			var sequence = Observable.Generate(
			  0,
			  i => i < 10000,
			  i => i * i + 1,
			  i => Math.Sqrt(i),
			  i => i == 0 ? dueTime : period
			); // infinite sequences
			using (sequence.Subscribe(WriteLine, andWeAreDone))
			{
				PressAnyKey();
			}


			WriteLine("this Observable.Range simply produces values from 10 to 15");
			var tenToTwenty = Observable.Range(10, 6);
			tenToTwenty.Subscribe(WriteLine, andWeAreDone);
			PressAnyKey();


			WriteLine("another Observable.Generate");
			var generated = Observable.Generate(1, // first value yielded
				value => value < 100, // constrain generation (limiting value)
				value => value * value + 1, // iteration step
				value => $"[{value}]"); // the Select() on the end
			generated.Subscribe(WriteLine, andWeAreDone);
			PressAnyKey();


			WriteLine("and another example of Observable.Interval");
			// Observable.Interval pushes incremental time values
			// Observable.Timer is like a Objctive.Interval that does only one value
			var interval = Observable.Interval(TimeSpan.FromMilliseconds(500));
			using (interval.Subscribe(WriteLine, andWeAreDone))
			{
				PressAnyKey();
			}
		}

		class Market : INotifyPropertyChanged
		{
			private float price;

			public float Price
			{
				get => price;
				set
				{
					if (value.Equals(price)) return;
					price = value;
					OnPropertyChanged();
				}
			}

			public void ChangePrice(float newPrice)
			{
				PriceChanged?.Invoke(this, newPrice);
			}

			public event EventHandler<float> PriceChanged;
			public event PropertyChangedEventHandler PropertyChanged;

			protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// very nice set of examples on how to opaque regular .net things with a observable paradigm
		public static void Converting_Into_Observables()
		{
			// observable from a list
			var items = new List<int> { 1, 2, 3 }; // blocking
			var source = items.ToObservable();
			source.Subscribe(WriteLine);
			PressAnyKey();


			// observable from a task
			var t = Task.Factory.StartNew(() => "Test");
			var source2 = t.ToObservable();
			source2.Subscribe(WriteLine, () => WriteLine("Task is done!"));
			PressAnyKey();
			

			// Observable.Start lets you turn a long running func or action into a reactive seq
			// when u call observable.Start is like a new thread
			var start = Observable.Start(() => // ThreadPool.QUWI
			{
				WriteLine("Starting work...");
				for (int i = 0; i < 10; i++)
				{
					Thread.Sleep(200);
					Write(".");
				}
			}); // this is done asynchronously on a thread pool thread

			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(200);
				Write("-");
			}

			start.Subscribe(
			  unit => WriteLine("Got a unit"),
			  () => WriteLine("Action complete")
			);
			PressAnyKey();

			// difference between observable.start and return:
			// observable.return is eager, observable.start is lazy

			// the point of Observable.Start is to integrate computationally heavy
			// work into a code base made up of observable sequences


			// observable from an event
			var market = new Market();
			var priceChanges = Observable.FromEventPattern<float>(
			  h => market.PriceChanged += h,
			  h => market.PriceChanged -= h
			); // also handle EventHandler w/o type

			// EventPattern<float>
			priceChanges.Subscribe(ep => WriteLine(ep.EventArgs));

			market.ChangePrice(1);
			market.ChangePrice(1.1f);
			market.ChangePrice(1.2f);
		}

		public static void Filtering()
		{
			// operators are same as LINQ
			WriteLine("---Observable.Range");
			var values = Observable.Range(-5, 11); // <-5, 5>
			values
			  .Select(x => x * x)
			  .Distinct()
			  .Subscribe(WriteLine);


			WriteLine(Environment.NewLine + "---List.ToObservable");
			new List<int> { 1, 1, 2, 2, 3, 3, 2, 2 }
			  .ToObservable()
			  .DistinctUntilChanged() // will return 1, 2, 3, 2
			  //.IgnoreElements() // ignores actual elements, same as Where(x => false)
			  .Subscribe(WriteLine,	() => WriteLine("Completed"));


			WriteLine(Environment.NewLine + "---Observable.Range Skip Take");
			Observable.Range(1, 5)
			  .Skip(1)
			  .Take(2)
			  .Subscribe(WriteLine);


			WriteLine(Environment.NewLine + "---Observable.Range SkipWhile TakeWhile");
			values.SkipWhile(x => x < 0)
			  .TakeWhile(x => x < 6)
			  .Subscribe(WriteLine);


			WriteLine(Environment.NewLine + "---Observable.Range SkipLast");
			values.SkipLast(5)
				.Subscribe(WriteLine); // caches all the values


			WriteLine(Environment.NewLine + "---Subject SkipUntil");
			// A and B are sequences
			// A will either skip or take the values until B produces a value
			var stockPrices = new Subject<float>();
			var optionPrices = new Subject<float>();
			
			// don't care about stock prices until option prices also available
			stockPrices.SkipUntil(optionPrices)
			  .Subscribe(WriteLine);
			stockPrices.OnNext(1);
			stockPrices.OnNext(2);
			stockPrices.OnNext(3);
			optionPrices.OnNext(55);
			stockPrices.OnNext(4);
			stockPrices.OnNext(5);
			stockPrices.OnNext(6);
		}

		public static void Sequence_Inspection()
		{
			// Any - true or false depending on whether the sequence has any elements
			// this means we have to wait for the sequence to complete before making the
			// termination

			var subject = new Subject<int>();

			subject.Any(x => x > 1) // later add predicate
				.Subscribe(	x => WriteLine($"Did we get any values? {x}"));

			subject.OnNext(1); // later
			WriteLine("Let's post another one");
			subject.OnNext(2);

			subject.OnCompleted();

			var values = new List<int> { 1, 2, 3, 4, 5 };
			values.ToObservable() // ToObservable calls OnNext on every element and OnCompleted
				.All(x => x > 0) // thats why All can give concrete result
				.Subscribe(WriteLine);

			var subj2 = new Subject<string>();

			subj2.Contains("bar")
				.Subscribe(x => WriteLine($"Does subject contain 'bar'? {x}"));

			subj2.OnNext("foo");
			//subj2.OnNext("bar");
			subj2.OnNext("baz");
			//subj2.OnCompleted(); // commenting out causes Does... to never be output

			var subj3 = new Subject<float>();
			subj3.DefaultIfEmpty(-99.9f) // note strict type adherence!
				.Subscribe(WriteLine);

			// later add
			subj3.OnNext(100.0f); // replaces -99.9f
			subj3.OnNext(101.0f);

			subj3.OnCompleted();

			var numbers = Observable.Range(1, 10);
			numbers.ElementAt(5) // try changing to 15
				.Subscribe(
				x => WriteLine($"The element at position 5 is {x}"),
				ex => WriteLine($"Could not get element because {ex.Message}"));

			// comparison of two sequences
			var seq1 = new Subject<int>();
			var seq2 = new Subject<int>();

			seq1.SequenceEqual(seq2)
				.Subscribe(x => WriteLine($"Are sequences equal? {x}"));

			seq1.Subscribe(x => WriteLine($"seq1 produces {x}"));
			seq2.Subscribe(x => WriteLine($"seq2 produces {x}"));

			seq1.OnNext(1);
			seq1.OnNext(2);

			seq2.OnNext(1);
			seq2.OnNext(2); // try changing to 3

			seq1.OnCompleted();
			seq2.OnCompleted(); // both sequences need to complete for result to be available
		}

		public static void Sequence_Transformation()
		{
			// select
			// cast + oftype
			// timestamp, timeinterval
			// materialize/dematerialize
			// selectmany

			var numbers = Observable.Range(1, 10);

			numbers.Select(x => x * x).Inspect("squares");

			var subj = new Subject<object>();

			subj.OfType<float>().Inspect("OfType"); // just filters out the right values
			subj.Cast<float>().Inspect("Cast");     // tries to cast every value to this type

			subj.OnNext(1.0f);
			subj.OnNext(2); // int
			subj.OnNext(3.0); // double

			// timestamp just posts the creation time
			//      var seq = Observable.Interval(TimeSpan.FromSeconds(1));
			//      seq.Timestamp().Inspect("Timestamp");
			//      seq.TimeInterval().Inspect("TimeInterval");

			//ReadKey();

			var seq2 = Observable.Range(0, 4);
			// Notification<int>
			seq2.Materialize()
			  //.Select(n => n.)
			  //.Select(n => n.Value)
			  .Dematerialize() // later
			  .Inspect("materialize");

			// SelectMany

			// normally elements are out of order
			// 1 1 2 1 2 3
			Observable.Range(1, 3) // try changing to 4
			  .SelectMany(x => Observable.Range(1, x))
			  .Inspect("gen");

			// this enforces the order
			Observable.Range(1, 4, ImmediateScheduler.Instance) // try changing to 4
			  .SelectMany(x => Observable.Range(1, x, ImmediateScheduler.Instance))
			  .Inspect("gen");
		}

		public static void Squence_Aggregation()
		{
			var values = Observable.Range(1, 5);
			values.Inspect("values");
			values.Count().Inspect("count");

			var intSubj = new Subject<int>();
			intSubj.Inspect("intSubj");
			intSubj.Min().Inspect("min"); // not a running minimum!
			intSubj.Sum().Inspect("sum"); // requires all values
			intSubj.Average().Inspect("avg"); // this automatically assumes the result is of type of double!

			intSubj.OnNext(2);
			intSubj.OnNext(4);
			intSubj.OnNext(1);
			intSubj.OnCompleted();

			// first, last, single
			var replay = new ReplaySubject<int>();

			// later
			replay.OnNext(-1); // doesn't complete unless OnCompleted
			replay.OnNext(2);

			replay.OnCompleted();

			replay.FirstAsync(i => i > 0).Inspect("FirstAsync"); // doesn't require completion

			replay.FirstOrDefaultAsync(i => i > 10) // yields 0 because no element matches
			  .Inspect("FirstOrDefaultAsync"); // doesn't require completion

			replay.SingleAsync().Inspect("SingleAsync"); // try commenting out one of OnNext
														 // requires completion! try commenting out OnCompleted -> no output

			//replay.SingleOrDefaultAsync()

			// Sum is always the final value, how about a running sum?
			var subj = new Subject<double>();
			int power = 1;

			subj.Aggregate(0.0, (p, c) => p + Math.Pow(c, power++)).Inspect("poly");
			subj.OnNext(1, 2, 4).OnCompleted(); // 1^1 + 2^2 + 4^3

			// running sum? no problem
			var subj2 = new Subject<double>();
			subj2.Scan(0.0, (p, c) => p + c).Inspect("scan"); // same as Aggregate().TakeLast()
			subj2.OnNext(1, 2, 3); //.OnCompleted();
								   // OnCompleted doesn't really matter anymore
		}
	}
}
