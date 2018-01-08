using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    public class ParallelLoops
    {
		// this function allows us to nicely convert foreach into a for with custom step
		static IEnumerable<int> Range(int start, int end, int step)
		{
			for (int i = start; i < end; i += step)
				yield return i;
		}
		
		public static void Paraller_Invoke_For_ForEach()
		{
			var a = new Action(() => Console.WriteLine($"First {Task.CurrentId}"));
			var b = new Action(() => Console.WriteLine($"Second {Task.CurrentId}"));
			var c = new Action(() => Console.WriteLine($"Third {Task.CurrentId}"));

			Parallel.Invoke(a, b, c);
			// these are blocking operations; wait on all tasks

			Parallel.For(1, 11, x =>
			{
				Console.Write($"{x * x}\t");
			});
			Console.WriteLine();

			// has a step strictly equal to 1
			// if you want something else...
			Parallel.ForEach(Range(1, 20, 3), Console.WriteLine);

			string[] letters = { "oh", "what", "a", "night" };
			var po = new ParallelOptions { MaxDegreeOfParallelism = 2 };
			Parallel.ForEach(letters, po, letter =>
			{
				Console.WriteLine($"{letter} has length {letter.Length} (task {Task.CurrentId})");
			});
		}

		public static void Breaking_Cancellations_And_Exceptions()
		{
			try
			{
				var cts = new CancellationTokenSource();
				var po = new ParallelOptions { CancellationToken = cts.Token };
				ParallelLoopResult result = Parallel.For(0, 20, po, (int i, ParallelLoopState state) =>
				{
					Console.WriteLine($"i={i} Task{Task.CurrentId}\t");
					if (i == 10)
					{
						// there are 4 ways of how we can stop break a loop
						cts.Cancel();
						//throw new Exception(); // execution stops on exception
						//state.Stop(); // stop execution as soon as possible
						//state.Break(); // request that loop stop execution of iterations beyond current iteration asap
					}
					if (state.IsExceptional)
						Console.WriteLine($"i={i} Task{Task.CurrentId} thrown an exception\t");

					// state.LowestBreakIteration, ShouldExitCurrentIteration
				});

				Console.WriteLine();
				Console.WriteLine($"Was loop completed? {result.IsCompleted}"); // uncomment break
				if (result.LowestBreakIteration.HasValue)
					Console.WriteLine($"Lowest break iteration: {result.LowestBreakIteration}");
			}
			catch (OperationCanceledException) { }
			catch (AggregateException ae)
			{
				ae.Handle(e =>
				{
					Console.WriteLine(e.Message);
					return true;
				});
			}
		}

		//private Random random = new Random();
		public static void Thread_Local_Storage()
		{
			// add numbers from 1 to 100

			int sum = 0;

			// parallel for is just a counter [start, finish)
			Parallel.For(1, 11, i =>
			{
				Console.Write($"[{i}] t={Task.CurrentId}\t");
				Interlocked.Add(ref sum, i); // concurrent access to sum from all these threads is inefficient
											 // all tasks can add up their respective values, then add sum to total sum
			});
			Console.WriteLine($"\nSum of 1..10 = {sum}");

			sum = 0;
			Parallel.For(1, 11,
			  () => 0, // initialize local state, show code completion for next arg
			  (x, state, currentValueTls) =>
			  {
				  currentValueTls += x;
				  Console.WriteLine($"Task {Task.CurrentId} has sum {currentValueTls}");
				  return currentValueTls;
			  },
			  partialSum =>
			  {
				  Console.WriteLine($"Partial value of task {Task.CurrentId} is {partialSum}");
				  Interlocked.Add(ref sum, partialSum);
			  }
			);
			Console.WriteLine($"Sum of 1..10 = {sum}");
		}

		[Benchmark]
		public void SquareEachValue()
		{
			const int count = 100000;
			var values = Enumerable.Range(0, count);
			var results = new int[count];

			// just like Paraller.For but for all the enumerables.
			// also Parallel process its task in a delegate which in this case is very inefficient.
			Parallel.ForEach(values, x => { results[x] = (int)Math.Pow(x, 2); });
		}

		[Benchmark]
		public void SquareEachValueChunked()
		{
			const int count = 100000;
			var results = new int[count];
			var part = Partitioner.Create(0, count, 10000); // rangeSize = size of each subrange

			Parallel.ForEach(part, range =>
			{
				for (int i = range.Item1; i < range.Item2; i++)
				{
					results[i] = (int)Math.Pow(i, 2);
				}
			});
		}
		
		public static void Partitioning_And_Benchmarking()
		{
			BenchmarkRunner.Run<ParallelLoops>();
		}
	}
}
