using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    class TaskProgramming
    {
        /// <summary>
        /// A unit of work. Task can only receive objects as parameters. 
        /// So boxing, unboxing etc happens all the time.
        /// </summary>
        public static void WriteTask(object o)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        public static int TextLength(object o)
        {
            Console.WriteLine($"\n Task with id {Task.CurrentId} processing object {o}...");
            return o.ToString().Length;
        }

        public static void Example_Of_Usage_Tasks_With_Return_Values()
        {
            string text1 = "testing", text2 = "this";
            var task1 = new Task<int>(TextLength, text1);
            task1.Start();

            Task<int> task2 = Task.Factory.StartNew(TextLength, text2);

            Console.WriteLine($"Length of '{text1}' is {task1.Result}");
            Console.WriteLine($"Length of '{text2}' is {task2.Result}");
        }

        public static void Example_Cancellation_Of_Task()
        {
            try
            {
                var cts = new CancellationTokenSource();
                var token = cts.Token;

                // to handle the situation when somebody cancel 
                token.Register(() =>
                {
                    Console.WriteLine("Job to be done when cancellation happened");
                });

                var t = new Task(() =>
                {
                    int i = 0;
                    while (true)
                    {
                        // this is one way so called soft break
                        //if (token.IsCancellationRequested) break;

                        // this is canonical way :)
                        //if (token.IsCancellationRequested)
                        //    throw new OperationCanceledException();

                        // which can be merged together (syntactic sugar) into
                        token.ThrowIfCancellationRequested();

                        // keep in mind that this exception will not be normally 
                        // so no catch statement is needed
                        // it will behave exactly the same as break but will also 
                        // leave a trace to make as know something canceled the task

                        Console.WriteLine($"{i++}\t");
                    }
                }, token);
                t.Start();

                Task.Factory.StartNew(() =>
                {
                    token.WaitHandle.WaitOne();
                    Console.WriteLine("Wait handle was released therefore cancellation was requested");
                });

                Console.ReadKey();
                cts.Cancel();
            }
            catch (Exception)
            {
                Console.WriteLine("We will never get here because OperationCanceledException is not a real exception.");
                throw;
            }
        }

        public static void Example_Several_Cancellation_Tokens()
        {
            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            // now no matter on which of the above token cancellation was requested 
            // it will cause paranoid because it is linked
            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
                planned.Token, preventative.Token, emergency.Token);

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"{i++}\t");
                    Thread.Sleep(1000);
                }
            }, paranoid.Token);

            Console.ReadKey();
            emergency.Cancel();
        }

        public static void Example_Wait_For_Time_To_Pass()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var t = new Task(() =>
            {
                Console.WriteLine("Press any key to disarm; you have 5 seconds.");
                var cancelled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(cancelled ? "Bomb disarmed." : "BOOM!!!");
            }, token);
            t.Start();

            Console.ReadKey();
            cts.Cancel();
        }

        public static void Example_Wait_For_Tasks()
        {
            var t = new Task(() =>
            {
                Console.WriteLine("I take 5 seconds.");

                for (int i = 0; i < 5; i++)
                {
                    // This not only pauses the execution but also tells the scheduler
                    // that this task does not need the processor time anymore so the scheduler
                    // can execute another task meanwhile
                    Thread.Sleep(1000);

                    // You also pause the thread but you do not give up your turn in overall execution
                    // rarely used only when we want to avoid context switching
                    //SpinWait.SpinUntil(() => false, 1000);
                }
                Console.WriteLine("I'm done.");
            });
            t.Start();

            Task t2 = Task.Factory.StartNew(() => Thread.Sleep(3000));

            // waits for all - if both were uncommented then only the last one takes action
            //Task.WaitAll(); 
            Task.WaitAny(new[] { t, t2 }, 4000); // waits for any or up to 4 seconds

            Console.WriteLine($"Task t status is {t.Status}");
            Console.WriteLine($"Task t2 status is {t2.Status}");
        }

        public static void Example_Handling_Exception()
        {
            try
            {
                var t = new Task(() => { throw new InvalidOperationException("Can't do this!") { Source = "t" }; });
                var t2 = new Task(() => { throw new FieldAccessException("Can't access this!") { Source = "t2" }; });
                t.Start();
                t2.Start();

                try
                {
                    Task.WaitAll(t, t2);
                }
                catch (AggregateException ae)
                {
                    // special type of exception specifically designed for TPL and 
                    // the idea is that you collect all the exceptions from all the tasks 
                    // you are working on and you put them into one exception
                    foreach (var e in ae.InnerExceptions)
                        Console.WriteLine($"Exception {e.GetType()} from {e.Source}");

                    ae.Handle(e =>
                    {
                        if (e is InvalidOperationException)
                        {
                            Console.WriteLine("Invalid Operation handled.");
                            return true; // exception handled
                        }
                        return false; // exception not handled meaning it will be thrown up the stack
                    });
                }
            }
            catch (AggregateException ae)
            {
                // all exceptions that manage to get there are handled
                foreach (var e in ae.InnerExceptions)
                    Console.WriteLine($"Exception {e.GetType()} from {e.Source}");
            }
        }
    }
}
