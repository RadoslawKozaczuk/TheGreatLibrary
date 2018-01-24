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

	class Course
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Category Category { get; set; }
		public CourseLevel Level { get; set; }
		public float FullPrice { get; set; }
		public Author Author { get; set; }

		public Course()
		{
			
		}

		public Course(string title, string description, Category category, CourseLevel level, float fullPrice, Author author)
		{
			Title = title;
			Description = description;
			Category = category;
			Level = level;
			FullPrice = fullPrice;
			Author = author;
		}

		public ICollection<CourseTag> PostTags { get; } = new List<CourseTag>();
	}
}
