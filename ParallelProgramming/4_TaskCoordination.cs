using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
	class TaskCoordination
	{
		public static void Continuations()
		{
			var taskBoil = Task.Factory.StartNew(() =>
			{
				Console.WriteLine($"Boil water (task {Task.CurrentId}, then...");
				throw null;
			});

			var taskPour = taskBoil.ContinueWith(t =>
			{
				// alternatively can also rethrow exceptions
				if (t.IsFaulted)
					throw t.Exception.InnerException;

				Console.WriteLine($"{t.Id} is {t.Status}, so pour into cup  {Task.CurrentId})");
			}, TaskContinuationOptions.NotOnFaulted);

			try
			{
				taskPour.Wait();
			}
			catch (AggregateException ae)
			{
				ae.Handle(e =>
				{
					Console.WriteLine("Exception: " + e);
					return true;
				});
			}

			var task1 = Task.Factory.StartNew(() => "Task 1");
			var task2 = Task.Factory.StartNew(() => "Task 2");
			var task3 = Task.Factory.StartNew(() => "Task 3");

			// also ContinueWhenAny
			var taskContinuation = Task.Factory.ContinueWhenAll(
				new[] { task1, task2, task3 }, 
				tasks =>
				{
					Console.WriteLine("Tasks completed:");
					foreach (var t in tasks)
						Console.WriteLine(" - " + t.Result);
					Console.WriteLine("All tasks done");
				}
			);

			// Keep in mind that continuation task which is specified as NotOnFaulted 
			// will not occur if the original task has faulted meaning you are stuck forever.

			task1.Wait();
		}

		public static void Child_Tasks()
		{
			// There is really no difference when a task is put in the parent's body or simply after the body. 
			// So a task without the AttachedToParent option created in the other task's body will be still just another task.
			var parent = new Task(() =>
			{
				var child = new Task(() =>
				{
					Console.WriteLine("Child task starting...");
					Thread.Sleep(3000);
					Console.WriteLine("Child task finished.");
					//throw new Exception(); // we may also throw an exception here
				}, TaskCreationOptions.AttachedToParent);

				var failHandler = child.ContinueWith(t =>
				{
					Console.WriteLine($"Unfortunately, task {t.Id}'s state is {t.Status}");
				}, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted); // '|' operator is logical sum thats why we use it here instead of '&'

				var completionHandler = child.ContinueWith(t =>
				{
					Console.WriteLine($"Hooray, task {t.Id}'s state is {t.Status}");
				}, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);

				child.Start();

				Console.WriteLine("Parent task starting...");
				Thread.Sleep(1000);
				Console.WriteLine("Parent task finished.");
			});

			parent.Start();
			try
			{
				parent.Wait();
			}
			catch (AggregateException ae)
			{
				ae.Handle(e => true);
			}
		}

		public static void Barrier()
		{
			var barrier = new Barrier(2, b =>
			{
				Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished.");
			});

			var water = Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Putting the kettle on (takes a bit longer).");
				Thread.Sleep(2000);
				// signaling and waiting - signal increments the barrier's internal counter. The task is blocked until the specified number is reached (in this case 2).
				barrier.SignalAndWait();
				Console.WriteLine("Putting water into cup.");
				barrier.SignalAndWait();
				Console.WriteLine("Putting the kettle away.");
			});

			var cup = Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Finding the nicest tea cup (only takes a second).");
				barrier.SignalAndWait();
				Console.WriteLine("Adding tea.");
				barrier.SignalAndWait();
				Console.WriteLine("Adding sugar.");
			});

			var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, tasks =>
			{
				Console.WriteLine("Enjoy your cup of tea.");
			});

			tea.Wait(); // here we have to wait otherwise the execution would go immediately to Program.cs and write "all done here"!
		}

		public static void Countdown_Event()
		{
			int taskCount = 5;
			var cte = new CountdownEvent(taskCount);
			var random = new Random();

			var tasks = new Task[taskCount];
			for (int i = 0; i < taskCount; i++)
			{
				tasks[i] = Task.Factory.StartNew(() =>
				{
					Console.WriteLine($"Entering task {Task.CurrentId}.");
					Thread.Sleep(random.Next(3000));
					cte.Signal(); // also takes a signalcount
								  //cte.CurrentCount/InitialCount
					Console.WriteLine($"Exiting task {Task.CurrentId}.");
				});
			}

			var finalTask = Task.Factory.StartNew(() =>
			{
				Console.WriteLine($"Waiting for other tasks in task {Task.CurrentId}");
				cte.Wait();
				Console.WriteLine("All tasks completed.");
			});

			finalTask.Wait();
		}

		// basically reset events are like counters but with count = 1
		public static void Manual_Reset_Event()
		{
			var evt = new ManualResetEventSlim();
			var cts = new CancellationTokenSource();
			var token = cts.Token;

			Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Boiling water...");
				for (int i = 0; i < 30; i++)
				{
					token.ThrowIfCancellationRequested();
					Thread.Sleep(100);
				}
				Console.WriteLine("Water is ready.");
				evt.Set();
			}, token);

			var makeTea = Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Waiting for water...");
				evt.Wait(5000, token);
				Console.WriteLine("Here is your tea!");
				Console.WriteLine($"Is the event set? {evt.IsSet}");

				evt.Reset();
				evt.Wait(1000, token); // already set!
				Console.WriteLine("That was a nice cup of tea!");
			}, token);

			makeTea.Wait(token);
		}
		
		public static void Auto_Reset_Event()
		{
			var evt = new AutoResetEvent(false);

			evt.Set(); // ok, it's set

			evt.WaitOne(); // this is ok but, in auto, it causes a reset
			
			// because the flag is reset to false the WaitOne() method times out.
			Console.WriteLine(evt.WaitOne(1000) ? "Succeeded" : "Timed out");
		}

		// so basically semaphore is like a counter but can go both ways
		public static void Semaphore()
		{
			var semaphore = new SemaphoreSlim(2, 10);

			for (int i = 0; i < 20; ++i)
			{
				Task.Factory.StartNew(() =>
				{
					Console.WriteLine($"Entering task {Task.CurrentId}.");
					semaphore.Wait(); // ReleaseCount--
					Console.WriteLine($"Processing task {Task.CurrentId}.");
				});
			}

			while (semaphore.CurrentCount <= 2)
			{
				Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
				Console.ReadKey();
				semaphore.Release(2); // ReleaseCount += n
			}
		}
	}
}
