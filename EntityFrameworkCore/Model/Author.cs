using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Model
{
	class Author
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public IList<Course> Courses { get; set; }

		public Author()
		{
			
		}
	}
}
