using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tuxboard.Core.Data.Context
{
    public class TuxboardContextFactory : IDesignTimeDbContextFactory<TuxDbContext>
    {
        public TuxDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TuxDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=Tuxboard;Trusted_Connection=True;");

            return new TuxDbContext(optionsBuilder.Options);
        }
    }
}