using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaymentsService.Infrastructure;

namespace PaymentsService.Infrastructure
{
    // EF tools will use this to create the DbContext at design time
    public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
    {
        public PaymentsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PaymentsDbContext>();

            // Use env var if present, otherwise fallback to localhost Postgres (Docker)
            var conn = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULT")
                       ?? "Host=localhost;Port=5432;Database=payments;Username=postgres;Password=postgres";

            builder.UseNpgsql(conn, npgsqlOptions =>
            {
                // optional Npgsql-specific options
                npgsqlOptions.EnableRetryOnFailure();
            });

            return new PaymentsDbContext(builder.Options);
        }
    }
}
