using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Model
{
	// EF Core 2.0 does not allow many to many relations as yet
	class CourseTag
    {
	    [Key]
		public int CourseTagId { get; set; }
		public int CourseId { get; set; }
	    public Course Course { get; set; }

	    public int TagId { get; set; }
	    public Tag Tag { get; set; }

	    public CourseTag()
	    {
		    
	    }
	}
}
