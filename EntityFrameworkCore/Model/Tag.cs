using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Model
{
	class Tag
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }

		public ICollection<CourseTag> PostTags { get; } = new List<CourseTag>();

		public Tag()
		{
			
		}
	}
}
