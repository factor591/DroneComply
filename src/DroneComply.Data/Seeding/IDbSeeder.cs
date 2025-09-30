using System.Threading;
using System.Threading.Tasks;

namespace DroneComply.Data.Seeding;

public interface IDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
