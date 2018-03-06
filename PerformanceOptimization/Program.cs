using System;

namespace PerformanceOptimization
{
    class Program
    {
		static void Main()
        {
			// wait to eliminate any startup overhead
			System.Threading.Thread.Sleep(500);

			//BasicPerformanceOptimizations.ImmutableStringExample();
			//BasicPerformanceOptimizations.BasicCilCode();
			//BasicPerformanceOptimizations.AvoidingBoxing();
			//BasicPerformanceOptimizations.UsingStringBuilder();
			//BasicPerformanceOptimizations.CollectionPerformanceExample();
			//BasicPerformanceOptimizations.ArraysExample();
			//BasicPerformanceOptimizations.ExceptionsExample();
			//BasicPerformanceOptimizations.ParseVsTryParse();
	        //BasicPerformanceOptimizations.ContainsKeyVsKeyNotFoundException();
	        //BasicPerformanceOptimizations.ForVersusForeach();

			//Exercises.RunExercises();
			//ItermediatePerformanceOptimizations.FastGarbageCollection();
	        //ItermediatePerformanceOptimizations.FastDelegates();
			ItermediatePerformanceOptimizations.ClassFactories();

	        Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
	        Console.ReadKey();
		}
    }
}