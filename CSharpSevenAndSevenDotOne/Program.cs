using System;

namespace CSharpSevenAndSevenDotOne
{
    class Program
    {
        static void Main(string[] args)
        {
			//Chapter 1 - C# 7
			//CSharpSeven.OutVariables();
			//CSharpSeven.Pattern_Maching();
			//CSharpSeven.Tuples();
			//CSharpSeven.Local_Functions();
			//CSharpSeven.Ref_Returns_And_Locals();
			//CSharpSeven.Expression_Bodied_Members();
			//CSharpSeven.Throw_Expressions();
			//CSharpSeven.Generalized_Async_Return_Types();
			//CSharpSeven.Literal_Improvments();

			// Chapter 2 - C# 7.1
			//CSharpSevenDotOne.Async_Main();
			//CSharpSevenDotOne.Default_Expressions();
			//CSharpSevenDotOne.Ref_Assemblies();
			CSharpSevenDotOne.Infer_Tuple_Names();

			Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
			Console.ReadKey();
		}
    }
}
