using System;

namespace ParallelProgramming
{
    class Program
    {
        static void Main(string[] args)
        {
			// Chapter 1 - Task Programming
			//TaskProgramming.Example_Of_Usage_Tasks_With_Return_Values();
			//TaskProgramming.Example_Cancellation_Of_Task();
			//TaskProgramming.Example_Several_Cancellation_Tokens();
			//TaskProgramming.Example_Wait_For_Time_To_Pass();
			//TaskProgramming.Example_Wait_For_Tasks();
			//TaskProgramming.Example_Handling_Exception();

			// Chapter 2 - Data Sharing And Synchronization
			//DataSharingAndSynchronization.Critical_Sections();
			//DataSharingAndSynchronization.Interlocked_Operations();
			//DataSharingAndSynchronization.Spin_Locking();
			//DataSharingAndSynchronization.Lock_Recursion(5);
			//DataSharingAndSynchronization.Local_Mutex();
			//DataSharingAndSynchronization.Global_Mutex();
			//DataSharingAndSynchronization.Reader_Writer_Locks();

			// Chapter 3 - Concurrent Collections
			//ConcurrentCollections.Concurrent_Dictionary();
			//ConcurrentCollections.Concurrent_Queue();
			//ConcurrentCollections.Concurrent_Stack();
			//ConcurrentCollections.Concurrent_Bag();
			//ConcurrentCollections.Blocking_Collection();

			//Chapter 4 - Task Coordination
			//TaskCoordination.Continuations();
			//TaskCoordination.Child_Tasks(); // this doesn't work as expected 
			//TaskCoordination.Barrier();
			//TaskCoordination.Countdown_Event();
			//TaskCoordination.Manual_Reset_Event();
			//TaskCoordination.Auto_Reset_Event();
			//TaskCoordination.Semaphore(); // I don't understand why it can open 20 even tho it has a limit to 10

			//Chapter 5 - Parallel loops
			//ParallelLoops.Paraller_Invoke_For_ForEach();
			//ParallelLoops.Breaking_Cancellations_And_Exceptions();
			//ParallelLoops.Thread_Local_Storage();
			//ParallelLoops.Partitioning_And_Benchmarking();

			//Chapter 6 - Parallel LINQ
			//ParallelLINQ.AsParallel_And_ParallelQuery();
			//ParallelLINQ.Cancellation_And_Exceptions();
			//ParallelLINQ.Merge();
			ParallelLINQ.Custom_Aggregation();

			Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
			Console.ReadKey();
        }
    }
}