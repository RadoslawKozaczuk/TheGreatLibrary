using System;

namespace CSharpSeven
{
    class Program
    {
        static void Main(string[] args)
        {
			//Chapter 1 - C# 7
			//CSharpSeven.OutVariables();
			//CSharpSeven.PatternMaching();
			//CSharpSeven.Tuples();
			//CSharpSeven.LocalFunctions();
			//CSharpSeven.RefReturnsAndLocals();
			//CSharpSeven.ExpressionBodiedMembers();
			//CSharpSeven.ThrowExpressions();
			//CSharpSeven.GeneralizedAsyncReturnTypes();
			//CSharpSeven.LiteralImprovments();

			// Chapter 2 - C# 7.1
			//CSharpSevenDotOne.AsyncMain();
			//CSharpSevenDotOne.DefaultExpressions();
			//CSharpSevenDotOne.RefAssemblies();
			//CSharpSevenDotOne.InferTupleNames();
	        //CSharpSevenDotOne.PatternMachingWithGenerics();

			// Chapter 3 - C# 7.2
			//CSharpSevenDotTwo.LeadingDigitSeparators();
			CSharpSevenDotTwo.PrivateProtected();

			Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
			Console.ReadKey();
		}
    }
}
