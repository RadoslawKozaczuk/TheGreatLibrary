using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading;
using System.Collections.Immutable;
using System.Reactive.Disposables;

namespace ReactiveExtensions
{
	public class Subjects
	{
		class DemoSubject : IObserver<float>
		{
			public DemoSubject()
			{
				var oo = new OperationObserver();

				// subject is both an observer and observable
				var market = new Subject<float>();

				// note also the overloads of this!
				market.Subscribe(this);
				//market.Subscribe(this); // it is possible to subscribe twice. All events will be duplicated.

				market.OnNext(123);
				market.Subscribe(oo); // late subscription
				market.OnNext(456);
				market.OnCompleted();

				market.OnNext(321.123f); // this will not work
				Console.WriteLine($"market.HasObservers? {market.HasObservers}"); // nope
			}

			public void OnNext(float value) => Console.WriteLine($"Market sent us {value}");

			public void OnError(Exception error) => Console.WriteLine($"We failed due to {error}");

			public void OnCompleted() => Console.WriteLine($"Market is closed");
		}

		class OperationObserver : IObserver<float>
		{
			public void OnNext(float value) => Console.WriteLine($"Operations normal, value = {value}");

			public void OnError(Exception error) => Console.WriteLine("Operations interrupted");

			public void OnCompleted() => Console.WriteLine("Operations complete");
		}


		// Subject<T> is both IObserver<T> and IObservable<T> and can act as a proxy between an observer and an observable
		public static void Subject() => new DemoSubject();

		public static void Unsubscription()
		{
			var sensor = new Subject<float>();

			// stick it in a using
			using (var subscription = sensor.Subscribe(Console.WriteLine))
			{
				sensor.OnNext(1);
				sensor.OnNext(2);
				sensor.OnNext(3);
			}

			// or dispose it
			//subscription.Dispose();

			sensor.OnNext(4); // this will make no effect because they are called after unsubscription
			sensor.OnNext(5);
		}

		public static void Proxy_And_Broadcast()
		{
			var market = new Subject<float>(); // observable
			var marketConsumer = new Subject<float>(); // observer and observable

			// isn't this API better?
			marketConsumer.SubscribeTo(market);

			// well, not really, because we need to be able to do this
			market
			  .Where(x => x > 2)
			  .Subscribe(marketConsumer);

			market.Inspect("market name"); // broadcasting

			// market -----> marketConsumer
			// we subscribe to marketConsumer
			marketConsumer.Subscribe(Console.WriteLine);

			// now post something on the market
			market.OnNext(1, 2, 3, 4);
			market.OnCompleted();
		}

		class DemoReplaySubject : IObserver<int>
		{
			public DemoReplaySubject()
			{
				var timeWindow = TimeSpan.FromMilliseconds(2000); // this will store everything that come in the next 2s
				//var market = new ReplaySubject<float>(2); // this will store only 2 last messages
				var market = new ReplaySubject<int>(timeWindow);
				market.OnNext(111);
				Thread.Sleep(800);
				market.OnNext(222);
				Thread.Sleep(800);
				market.OnNext(333);
				Thread.Sleep(800);

				market.Subscribe(this);
				market.OnNext(444);
			}

			public void OnNext(int value) => Console.WriteLine($"I got the value {value}");

			public void OnError(Exception error) => throw new NotImplementedException();

			public void OnCompleted() => throw new NotImplementedException();
		}

		public static void Replay_Subject() => new DemoReplaySubject();

		// the only difference between BehavioSubject and Subject is that the first has a default value
		public static void Behavior_Subject()
		{
			var sensorReading = new BehaviorSubject<double>(-1.0); 
			sensorReading.Subscribe(Console.WriteLine);
			sensorReading.OnNext(0.98);
		}

		public static void Async_Subject()
		{
			// AsyncSubject always stores the last value, and only
			// gives it up on OnCompleted
			AsyncSubject<double> sensor = new AsyncSubject<double>();
			sensor.OnNext(10);
			sensor.Subscribe(Console.WriteLine);
			sensor.OnNext(20);
			sensor.OnCompleted();

			// implicit contact - sequence ends in either
			// OnCompleted or OnError
			sensor.OnNext(30); // does nothing
		}

		class Market : IObservable<float>
		{
			private ImmutableHashSet<IObserver<float>> observers = ImmutableHashSet<IObserver<float>>.Empty;

			public IDisposable Subscribe(IObserver<float> observer)
			{
				observers = observers.Add(observer);
				return Disposable.Create(() =>
				{
					observers = observers.Remove(observer);
				});
			}

			public void Publish(float price)
			{
				foreach (var o in observers)
					o.OnNext(price);
			}
		}

		public static void Implementing_IObservable()
		{
			var market = new Market();
			var subscription = market.Subscribe(value => Console.WriteLine($"Got market value {value}"));

			market.Publish(123);
		}
	}
}