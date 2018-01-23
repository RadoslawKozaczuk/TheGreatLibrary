using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EntityFrameworkCore.DbContext
{
	public class PlutoContextFactory : IDesignTimeDbContextFactory<PlutoContext>
	{
		PlutoContext IDesignTimeDbContextFactory<PlutoContext>.CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<PlutoContext>();
			return new PlutoContext(builder.Options);
		}
	}
}
