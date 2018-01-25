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

	class Course
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
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
