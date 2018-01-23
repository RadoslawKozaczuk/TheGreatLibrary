using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EntityFrameworkCore.DbContext
{
	public class PlutoContextFactory : IDesignTimeDbContextFactory<PlutoContext>
	{
		PlutoContext IDesignTimeDbContextFactory<PlutoContext>.CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<PlutoContext>();
			optionsBuilder.UseInMemoryDatabase("PlutoDb");

			return new PlutoContext(optionsBuilder.Options);
		}
	}
}
