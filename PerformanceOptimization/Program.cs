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
	        BasicPerformanceOptimizations.ForVersusForeach();

			Console.ReadLine();
        }
    }
}