using System;

//Reminder - to install DB:
// - install SQL server (engine), it additionally contains SQL Management Studio
// - add ADO .NET Entity Data Model to the project

// updating a model is very easy just open .edmx file and click "Update Model from Database"
// editing and removing columns may cause errors in conceptual models that later have to be fixed manually
// also it sometimes not change the data type especially if the new type will always compile due to save conversion
// f.e. we had a tinyInt (byte) and we changed to smallInt (Int16)
// same if we delete a table from a database we may need to manually delete it from the conceptual model

/* === Complex Types and Entities
	Stored Procedures may return None, Scalar, Complex type or an Entity
	Complex type is just a class that is not present in the model
	We use it for example when an SP is returning a result of joining two tables
	We can easily create our own ComplexTypes 
*/

namespace EntityFramework
{
	class Program
	{
		/*  === Enums ==
			Enums can be add in two ways.
			First is to add enum value in the Entity Model Browser and then simply chose it as a field value type.
			Another way is to use enum from the Code like the one below and then add it to the Enums in the Model Browser.
		*/
		public enum Level : byte
		{
			Beginner = 1,
			Intermediate = 2,
			Advanced
		}

		static void Main()
		{
			var dbContext = new PlutoDbContext();
			var courses = dbContext.GetCourses();

			foreach (var course in courses)
			{
				Console.WriteLine(course.Title);
			}

			Console.WriteLine(Environment.NewLine + "All done here. Press any key to exit.");
			Console.ReadKey();
		}
	}
}
