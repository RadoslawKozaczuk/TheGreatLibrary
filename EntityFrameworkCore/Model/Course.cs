using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Model
{
	public enum CourseLevel
	{
		Beginner = 1,
		Intermediate = 2,
		Advanced = 3
	}

	// framework assumes all tables are in .dbo schema
	//[Table("someTable", Schema = "mySchema")] // write this to map to another table or schema
	class Course
	{
		// by default primary key is ID or ClassNameID
		// but Id is not cool man, that's illegal, you may go to jail for that
		[Key] // bribe the policeman with this annotation
		public int Id { get; set; }
		// by default strings are mapped to nvarchar
		//[Column("columnName", TypeName = "varchar")] // write this to map to another column or a different type
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		public DateTime? DatePublished { get; set; }
		public CourseLevel Level { get; set; }
		public float FullPrice { get; set; }
		public Author Author { get; set; }

		public Course()
		{
			
		}

		public Course(string name, string description, CourseLevel level, float fullPrice, Author author)
		{
			Name = name;
			Description = description;
			Level = level;
			FullPrice = fullPrice;
			Author = author;
		}

		public ICollection<CourseTag> PostTags { get; } = new List<CourseTag>();
	}
}
