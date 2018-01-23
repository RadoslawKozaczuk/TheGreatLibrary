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
			// in case db does not exist run "Update-Database" command in the Package Manager Console
			optionsBuilder.UseSqlServer(@"Server=localhost;Database=PlutoCodeFirst;Trusted_Connection=True");
			
			// in memory database
			//optionsBuilder.UseInMemoryDatabase("PlutoInMemoryDb");
		}
	}
}
