using EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.DbContext
{
	class PlutoContext : Microsoft.EntityFrameworkCore.DbContext
	{
		public PlutoContext(DbContextOptions<PlutoContext> options)
			: base(options)
		{

		}

		public DbSet<Course> Courses { get; set; }
		public DbSet<Author> Authors { get; set; }
		public DbSet<Tag> Tags { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseInMemoryDatabase("PlutoDb");
		}
	}
}
