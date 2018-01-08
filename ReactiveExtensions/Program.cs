using System;

namespace ReactiveExtensions
{
	/*
	 Reactive Extensions is a technology for working with streams of data in a reactive fashion. 
	 With Reactive Extensions, you can ingest, filter analyze and re-broadcast data streams. 
	 You can also build your own components which generate reactive streams for others to produce.
	 */
	class Program
    {
        static void Main(string[] args)
        {
			// Chapter 1 - Subjects
			//Subjects.Subject();
			//Subjects.Unsubscription();
			//Subjects.Proxy_And_Broadcast();
			//Subjects.Replay_Subject();
			//Subjects.Behavior_Subject();
			//Subjects.Async_Subject();
			//Subjects.Implementing_IObservable();


			// Chapter 2 - Fundamental Sequence Operators
			//FundamentalSequenceOperators.Observable_Create();
			//FundamentalSequenceOperators.Sequence_Generators();
			//FundamentalSequenceOperators.Converting_Into_Observables();
			//FundamentalSequenceOperators.Filtering();
			//FundamentalSequenceOperators.Sequence_Inspection(); // didn't have time to check that out
			//FundamentalSequenceOperators.Sequence_Transformation(); // didn't have time to check that out
			//FundamentalSequenceOperators.Squence_Aggregation(); // didn't have time to check that out


			// Chapter 3 - Advanced Sequence Operators
			//AdvancedSequenceOperators.Exeception_Handling();
			//AdvancedSequenceOperators.Sequence_Combinators();
			//AdvancedSequenceOperators.Time_Related_Sequence_Processing();
			AdvancedSequenceOperators.Reactive_Extensions_Event_Broker_With_Autofac();


			Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
			Console.ReadKey();
		}
    }
}
