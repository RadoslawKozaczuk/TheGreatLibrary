using System;
using static System.Console;

namespace CSharpSeven
{
    class CSharpConcepts
    {
        class BaseClass
        {
            public virtual string Method1() => "BaseMethod1";
            public virtual string Method2() => "BaseMethod2";
            public string Method3() => "BaseMethod3";
            public string Method4() => "BaseMethod4";
        }

        class DerivedClass : BaseClass
        {
            public override string Method1() => "DerivedMethod1";
            // new keyword is only to suppress the warning and indicate to the editor that hiding was intended
            public new string Method2() => "DerivedMethod2";
            public new string Method3() => "DerivedMethod3";

            // not possible - we cannot override a member that is not marked virtual in the base class
            //public override string Method4() => "DerivedMethod4";
        }

        public static void OverrideAndNew()
        {
            BaseClass @base = new BaseClass();
            DerivedClass derived = new DerivedClass();
            BaseClass baseDerived = new DerivedClass();
            //DerivedClass derivedBase = new BaseClass(); // impossible - cannot implicitly convert
            //DerivedClass derivedBase = (DerivedClass)new BaseClass(); // also impossible - runtime error

            // The following two calls do what you would expect. They call the methods that are defined in BaseClass.  
            WriteLine("base.Method1(): " + @base.Method1());
            WriteLine("base.Method2(): " + @base.Method2());
            WriteLine("base.Method3(): " + @base.Method3());
            WriteLine();

            // The following two calls do what you would expect. They call the methods that are defined in DerivedClass.  
            WriteLine("derived.Method1(): " + derived.Method1());
            WriteLine("derived.Method2(): " + derived.Method2());
            WriteLine("derived.Method3(): " + derived.Method3());
            WriteLine();

            // Here only the first one calls the DerivedClass method.
            WriteLine("baseDerived.Method1(): " + baseDerived.Method1());
            WriteLine("baseDerived.Method2(): " + baseDerived.Method2());
            WriteLine("baseDerived.Method3(): " + baseDerived.Method3());
            WriteLine();

            // In other words when the method is hidden it is still accessible through casting.
            WriteLine("((BaseClass)derived).Method2(): " + ((BaseClass)derived).Method2());
            WriteLine("derived.Method2(): " + derived.Method2());

            ReadKey();
        }

        enum MathOperations { Addition, Subtraction, Multiplication, Division }
        delegate int MathFunction(int a, int b);

        class Calculator
        {
            public MathFunction function;

            public int Calculate(int a, int b, MathFunction function) => function(a, b);
            public int Calculate(int a, int b, MathOperations operation)
            {
                AssignOperation(operation);
                return function(a, b);
            }

            public void AssignOperation(MathOperations operation)
            {
                switch(operation)
                {
                    case MathOperations.Addition:
                        // we can use anonymous delegate
                        function = delegate (int a, int b)
                        {
                            return a + b;
                        };
                        break;
                    case MathOperations.Subtraction:
                        // we can also use lambda expression syntax
                        function = (int a, int b) =>
                        {
                            return a - b;
                        };
                        break;
                    case MathOperations.Multiplication:
                        // lambda expressions can use parameter type inference for more readability
                        // and obviously also body expressions
                        function = (a, b) => a * b;
                        break;
                    case MathOperations.Division:
                        // unfortunately we cannot convert Func into our custom delegate even if the signature matches
                        // Func<int, int, int> myFunc = (a, b) => a / b;
                        // function = (MathFunction)myFunc; // error
                        function = (a, b) => a / b;
                        break;
                }
            }
        }
        
        public static void FunctionalProgramming()
        {
            var calc = new Calculator();
            WriteLine(calc.Calculate(2, 2, MathOperations.Addition));
            WriteLine(calc.Calculate(10, 0, (a, b) => a * a));
            
            // there is also something called local functions and we can use them as well
            int LocalAdd(int a, int b) => a + b;
            WriteLine(calc.Calculate(1, 1, LocalAdd));
            WriteLine();

            // Func, Action and Predicate are all built-in custom delegate types
            Func<int> getRandomNumber = () => new Random().Next(2, 2); // Func must return something
            Predicate<int> isEven = delegate (int n) { return n % 2 == 0; }; // Predicate returns bool
            Action<bool> writeResult = n => WriteLine(n); // Action returns void

            writeResult.Invoke(isEven.Invoke(getRandomNumber.Invoke()));
            // Invoke can be omitted for simplicity if it does not make confusion
            writeResult(isEven(getRandomNumber()));
        }
    }
}