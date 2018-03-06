using System;
using System.Diagnostics;

namespace PerformanceOptimization
{
	class AdvancedPerformanceOptimization
	{
		const int Repetitions = 1000;

		static long ArrayMeasure(int elements)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			int[] list = new int[elements];
			for (int r = 0; r < Repetitions; r++)
			{
				for (int i = 0; i < elements; i++)
				{
					list[i] = i;
				}
			}

			stopwatch.Stop();
			return stopwatch.ElapsedTicks;
		}

		static long StackallocMeasure(int elements)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			unsafe
			{
				int* list = stackalloc int[elements];
				for (int r = 0; r < Repetitions; r++)
				{
					for (int i = 0; i < elements; i++)
					{
						list[i] = i;
					}
				}
			}

			stopwatch.Stop();
			return stopwatch.ElapsedTicks;
		}

		public static void ArrayOnTheStack()
		{
			// Note: stack size equals to 1 MB for 32-bit processes and 4 MB for 64-bit processes
			Console.WriteLine("ele\tstalloc\tint[]");

			long sumArray = 0;
			long sumStackalloc = 0;

			// measurement run
			for (int elements = 10_000; elements <= 100_000; elements += 10_000)
			{
				long duration1 = ArrayMeasure(elements);
				long duration2 = StackallocMeasure(elements);
				Console.WriteLine($"{elements}\t{duration1}\t{duration2}");

				sumArray += duration1;
				sumStackalloc += duration2;
			}

			Console.WriteLine($"Stackallock performance as a % of the int[] { sumStackalloc * 100 / sumArray }");

			/* Results:
				- stackalloc is barely better. Taking into consideration its drawbacks it should be not used at all.
				- stackalloc is present in the language as a low level interface (gateway) to other languages
			 */
		}
	}
}