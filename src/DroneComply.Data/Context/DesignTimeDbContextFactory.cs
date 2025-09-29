using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DroneComply.Data.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DroneComplyDbContext>
{
    public DroneComplyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DroneComplyDbContext>();
        optionsBuilder.UseSqlite("Data Source=DroneComply.db");
        return new DroneComplyDbContext(optionsBuilder.Options);
    }
}
