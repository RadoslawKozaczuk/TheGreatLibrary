using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    class BankAccount
    {
        public object padlock = new object();
        public int Balance { get; private set; }
        
        // these operations are not atomic thats why we 
        // need to lock access to this operation in order to not 
        // get different result every single time
        public void Deposit(int amount)
        {
            // +=
            // op1: temp <- get_Balance() + amount
            // op2: set_Balance(temp)
            lock (padlock)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            // lock construct is a shorthand for Monitor.Enter and Monitor.Exit
            Monitor.Enter(padlock);
            Balance -= amount;
            Monitor.Exit(padlock);

            // there are also different methods like for example TryEnter which wait certain amount of time
            //Monitor.TryEnter(padlock, 1000);
        }
    }

    class BankAccountV2
    {
        public object padlock = new object();

        private int _balance;
        public int Balance
        {
            get
            {
                return _balance;
            }
            private set
            {
                _balance = value;
            }
        }
        
        public void Deposit(int amount)
        {
            // Add method takes ref argument
            // so we cannot use automatic property because it is impossible 
            // to access the backing field of an automatic property
            // but its not a big deal we just change the property to a normal 
            Interlocked.Add(ref _balance, amount); // this performs an atomic operation
        }

        public void Withdraw(int amount)
        {
            Interlocked.Add(ref _balance, -amount); // this performs an atomic operation
        }
    }

    class BankAccountV3
    {
        public int Balance { get; private set; }

        public BankAccountV3(int balance)
        {
            Balance = balance;
        }

        public void Deposit(int amount)
        {
            Balance += amount;
        }

        public void Withdraw(int amount)
        {
            Balance -= amount;
        }

        public void Transfer(BankAccountV3 where, int amount)
        {
            where.Balance += amount;
            Balance -= amount;
        }
    }

    class DataSharingAndSynchronization
    {
        public static void Critical_Sections()
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();

            for(int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Deposit(100);
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Withdraw(100);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
            Console.ReadKey();
        }

        public static void Interlocked_Operations()
        {
            var tasks = new List<Task>();
            var ba = new BankAccountV2();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Deposit(100);
                        
                        // Explanation:
                        // op 1
                        // op 2
                        //Interlocked.MemoryBarrier();
                        // op 3
                        // MemoryBarrier ensures that operations 1 and 2 will never be executed before operation 3
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Withdraw(100);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
            Console.ReadKey();
        }
        
        public static void Spin_Locking()
        {
            var tasks = new List<Task>();
            var ba = new BankAccountV3(0);

            SpinLock sl = new SpinLock();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        // previous examples assumed that lock was always possible to take
                        var lockTaken = false;
                        try
                        {
                            sl.Enter(ref lockTaken);
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken) sl.Exit();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            sl.Enter(ref lockTaken);
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken) sl.Exit();
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
            Console.ReadKey();
        }

        // the argument specifies whether the SpinLock should enable thread owner tracking
        // depending on whether you set this argument or not you are going to have an information
        // about which thread locked the particular SpinLock
        public static SpinLock sl2 = new SpinLock(true);

        // x is the depth of the recursion
        public static void Lock_Recursion(int x)
        {
            // lock recursion is being able to take the same lock multiple times
            bool lockTaken = false;
            try
            {
                sl2.Enter(ref lockTaken);
            }
            catch(LockRecursionException e)
            {
                Console.WriteLine("Exception: " + e);
            }
            finally
            {
                if(lockTaken)
                {
                    Console.WriteLine($"Took a lock, x = {x}");
                    Lock_Recursion(x - 1);
                    sl2.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take a lock, x = {x}");
                }
            }
        }

        public static void Local_Mutex()
        {
            var tasks = new List<Task>();
            var ba = new BankAccountV3(0);
            var ba2 = new BankAccountV3(0);

            // many synchro types deriving from WaitHandle
            // Mutex = mutual exclusion

            // two types of mutexes
            // this is a _local_ mutex
            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(1); // deposit 10000 overall
                        }
                        finally
                        {
                            if (haveLock) mutex.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            ba2.Deposit(1); // deposit 10000
                        }
                        finally
                        {
                            if (haveLock) mutex2.ReleaseMutex();
                        }
                    }
                }));

                // transfer needs to lock both accounts
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = Mutex.WaitAll(new[] { mutex, mutex2 });
                        try
                        {
                            ba.Transfer(ba2, 1); // transfer 10k from ba to ba2
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is: ba={ba.Balance}, ba2={ba2.Balance}.");
        }

        public static void Global_Mutex()
        {
            const string appName = "MyApp";
            Mutex mutex;
            try
            {
                mutex = Mutex.OpenExisting(appName);
                Console.WriteLine($"Sorry, {appName} is already running.");
                return;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Console.WriteLine("We can run the program just fine.");
                // first arg = whether to give current thread initial ownership
                mutex = new Mutex(false, appName);
            }

            Console.ReadKey();
        }

        // recursion is not recommended and can lead to deadlocks
        static ReaderWriterLockSlim padlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public static void Reader_Writer_Locks()
        {
            int x = 0;

            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    // entering two read locks is possible but it may get too complicated
                    padlock.EnterReadLock();
                    padlock.EnterReadLock();


                    //padlock.EnterUpgradeableReadLock();

                    //if (i % 2 == 0)
                    //{
                    //    padlock.EnterWriteLock();
                    //    x++;
                    //    padlock.ExitWriteLock();
                    //}

                    // can now read
                    Console.WriteLine($"Entered read lock, x = {x}, pausing for 5sec");
                    Thread.Sleep(5000);

                    // when two read locks were entered two have to be released
                    padlock.ExitReadLock();
                    padlock.ExitReadLock();


                    //padlock.ExitUpgradeableReadLock();

                    Console.WriteLine($"Exited read lock, x = {x}.");
                }));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
            }

            var random = new Random();

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) break;
                padlock.EnterWriteLock();
                Console.WriteLine("Write lock acquired");
                int newValue = random.Next(10);
                x = newValue;
                Console.WriteLine($"Set x = {x}");
                padlock.ExitWriteLock();
                Console.WriteLine("Write lock released");
            }
        }
    }
}
