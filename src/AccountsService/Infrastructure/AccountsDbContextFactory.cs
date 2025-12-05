using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AccountsService.Infrastructure
{
    public class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
    {
        public AccountsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AccountsDbContext>();

            var conn = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__ACCOUNTS")
                       ?? "Server=localhost,1433;Database=accounts;User Id=sa;Password=P@ssw0rd123!;TrustServerCertificate=True";

            builder.UseSqlServer(conn, sqlOptions => sqlOptions.EnableRetryOnFailure());
            return new AccountsDbContext(builder.Options);
        }
    }
}
