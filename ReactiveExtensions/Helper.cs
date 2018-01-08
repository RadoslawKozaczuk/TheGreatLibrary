using System;
using static System.Console;

namespace ReactiveExtensions
{
	// bunch of handy extension methods used during the course.
	static class ExtensionMethods
	{
		// inversion of control
		public static IDisposable SubscribeTo<T>(this IObserver<T> observer, IObservable<T> observable)
			=> observable.Subscribe(observer);

		public static IObserver<T> OnNext<T>(this IObserver<T> self, params T[] args)
		{
			foreach (var arg in args)
				self.OnNext(arg);
			return self;
		}

		public static IDisposable Inspect<T>(this IObservable<T> self, string name)
		{
			return self.Subscribe(
				x => WriteLine($"{name} has generated value {x}"),
				ex => WriteLine($"{name} has generated exception {ex.Message}"),
				() => WriteLine($"{name} has completed"));
		}
	}

	static class Helper
    {
		public static Action PressAnyKey = new Action(() =>
		{
			WriteLine("=== Press any key to continue" + Environment.NewLine);
			ReadKey();
		});
	}
}
