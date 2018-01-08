using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
	class ParallelLINQ
	{
		public static void AsParallel_And_ParallelQuery()
		{
			const int count = 10;

			// let's generate an array of numbers from 1 to 20
			var items = Enumerable.Range(1, count).ToArray();

			// now we can get the cubed value of each element in the array using
			Console.WriteLine("CalculatedValue (TaskId)");
			var results = new int[count];
			items.AsParallel().ForAll(x =>
			{
				int newValue = x * x * x;
				Console.Write($"{newValue} ({Task.CurrentId})    ");
				results[x - 1] = newValue;
			});
			Console.WriteLine();
			Console.WriteLine();

			foreach (var res in results)
				Console.Write($"{res}\t");
			Console.WriteLine();

			// now let's get an enumeration
			// by default, the sequence is quite different to our nicely laid out array
			// but....                    .AsOrdered()
			// also...                    .AsUnordered() <- default
			var cubes = items.AsParallel().AsOrdered().Select(x => x * x * x);

			// like with all LINQ the variable 'cubes' is just a plan and the actual execution happens when we iterate over
			// this evaluation is delayed: you actually calculate the values only
			// when iterating or doing ToArray()
			foreach (var i in cubes)
				Console.Write($"{i}\t");
			Console.WriteLine();
		}

		public static void Cancellation_And_Exceptions()
		{
			var cts = new CancellationTokenSource(); // as usual after cancellation we cannot threads to stop immediately some of them will continue 
			var items = Enumerable.Range(1, 20);

			var results = items.AsParallel()
			  .WithCancellation(cts.Token).Select(i =>
			  {
				  double result = Math.Log10(i);

				  //if (result > 1) throw new InvalidOperationException(); // in this case it would be aggregated exception

				  Thread.Sleep((int)(result * 1000));
				  Console.WriteLine($"i = {i}, tid = {Task.CurrentId}");
				  return result;
			  });

			// no exception yet, but...
			try
			{
				foreach (var c in results)
				{
					if (c > 1)
						cts.Cancel();
					Console.WriteLine($"result = {c}");
				}
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($"Canceled");
			}
			catch (AggregateException ae)
			{
				ae.Handle(e =>
				{
					Console.WriteLine($"{e.GetType().Name}: {e.Message}");
					return true;
				});
			}
		}

		// merge options essentially let you decide when you gt the results once they are calculated
		public static void Merge()
		{
			var numbers = Enumerable.Range(1, 20).ToArray();

			// FullyBuffered = all results produced before they are consumed
			// NotBuffered = each result can be consumed right after it's produced
			// Default = AutoBuffered = buffer the number of results selected by the runtime (the system decides how to do the buffer)
			var results = numbers.AsParallel()
			  .WithMergeOptions(ParallelMergeOptions.AutoBuffered)
			  .Select(x =>
			  {
				  Console.WriteLine($"+Produced {x}");
				  return x;
			  });

			foreach (var result in results)
				Console.WriteLine($"-Consumed {result}");
		}

		public static void Custom_Aggregation()
		{
			//Sum Average etc are special cases or more general operator - Aggregate
			
			// some operations in LINQ perform an aggregation
			//var sum = Enumerable.Range(1, 20).Sum();
			//var sum = ParallelEnumerable.Range(1, 20).Sum();
			
			// Sum is just a specialized case of Aggregate
			//var sum = Enumerable.Range(1, 20).Aggregate(0, (i, acc) => i + acc);

			var sum = ParallelEnumerable.Range(1, 20)
			  .Aggregate(
				  0, // initial seed for calculations
				  (partialSum, i) => partialSum += i, // per task
				  (total, subtotal) => total += subtotal, // combine all tasks
				  i => i); // final result processing

			Console.WriteLine($"Sum is {sum}");
		}
	}
}
