using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    class ConcurrentCollections
    {
        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

        public static void AddParis()
        {
            // there is no add, since you don't know if it will succeed
            bool success = capitals.TryAdd("France", "Paris");

            string who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "Main thread";
            Console.WriteLine($"{who} {(success ? "added" : "did not add")} the element.");
        }

        public static void Concurrent_Dictionary()
        {
            AddParis();
            Task.Factory.StartNew(AddParis).Wait();

            //capitals["Russia"] = "Leningrad";
            // first parameter is key, second is value in case of the element not being there, third is the value in case when the element is there
            var s = capitals.AddOrUpdate("Russia", "Moscow", (key, old) => old + " --> Moscow"); // if the element is already there update if not add it
            Console.WriteLine("The capital of Russia is " + capitals["Russia"]);

            capitals["Sweden"] = "Uppsala";
            var capOfNorway = capitals.GetOrAdd("Sweden", "Stockholm");
            Console.WriteLine($"The capital of Sweden is {capOfNorway}.");

            // removal
            const string toRemove = "Russia"; // make a mistake here
            Console.WriteLine(capitals.TryRemove(toRemove, out string removed) 
                ? $"We just removed {removed}" 
                : $"Failed to remove capital of {toRemove}");

            // some operations are slow, e.g.,
            Console.WriteLine($"The ");

            foreach (var kv in capitals)
            {
                Console.WriteLine($" - {kv.Value} is the capital of {kv.Key}");
            }

            Console.ReadKey();
        }

        // Concurrent Queue behaves in a very similar fashion to a normal queue but we have to use this is in multi-thread environment.
        public static void Concurrent_Queue()
        {
            var q = new ConcurrentQueue<int>();
            q.Enqueue(1);
            q.Enqueue(2);
			
            //int last = q.Dequeue();
            if (q.TryDequeue(out int result))
                Console.WriteLine($"Removed element {result}");
			
            //int peeked = q.Peek();
            if (q.TryPeek(out result))
                Console.WriteLine($"Last element is {result}");
        }

        // Another collection that got its concurrent counterpart is stack.
        public static void Concurrent_Stack()
        {
            var stack = new ConcurrentStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            if (stack.TryPeek(out int result))
                Console.WriteLine($"{result} is on top");

            if (stack.TryPop(out result))
                Console.WriteLine($"Popped {result}");

            var items = new int[5];
            if (stack.TryPopRange(items, 0, 5) > 0) // actually pops only 3 items
            {
                var text = string.Join(", ", items.Select(i => i.ToString()));
                Console.WriteLine($"Popped these items: {text}");
            }
        }

        // ConcurentBag is not ordered but in exchange optimized on speed.
        public static void Concurrent_Bag()
        {
            // stack is LIFO
            // queue is FIFO
            // concurrent bag provides NO ordering guarantees

            // keeps a separate list of items for each thread
            // typically requires no synchronization, unless a thread tries to remove an item
            // while the thread-local bag is empty (item stealing)
            var bag = new ConcurrentBag<int>();
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var number = i; // this is important we cannot just add i because all would get 10
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    bag.Add(number);
                    Console.WriteLine($"{Task.CurrentId} has added {number}");
                    if (bag.TryPeek(out int result))
                    {
                        Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // take whatever's last
            if (bag.TryTake(out int last))
            {
                Console.WriteLine($"Last element is {last}");
            }
        }

        static CancellationTokenSource cts = new CancellationTokenSource();

        private static void ProduceAndConsume()
        {
            // blocking collection is not a collection it is just a wrapper around some underline collection
            var messages = new BlockingCollection<int>(
                new ConcurrentBag<int>(),
                10 /* limit set to 10 */
            );
            
            var random = new Random();

            var producer = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    int i = random.Next(100);
                    messages.Add(i);
                    Console.WriteLine($"added: {i} (count = {messages.Count}"); // count can block and make collection slow
                    Thread.Sleep(random.Next(500));
                }
            });

            var consumer = Task.Factory.StartNew(() =>
            {
                foreach (var item in messages.GetConsumingEnumerable())
                {
                    cts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"processed: {item}");
                    Thread.Sleep(random.Next(1000));
                }
            });

            try
            {
                Task.WaitAll(new[] { producer, consumer }, cts.Token);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => true);
            }
        }

        public static void Blocking_Collection()
        {
            // here we wrap it up in another Task to able to nicely stop both with a single cancellation token
            Task.Factory.StartNew(ProduceAndConsume, cts.Token);

            Console.ReadKey();
            cts.Cancel();
        }
    }
}
