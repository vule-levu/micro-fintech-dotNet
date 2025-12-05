using Microsoft.EntityFrameworkCore;
using AccountsService.Domain.Entities;

namespace AccountsService.Infrastructure
{
    public class AccountsDbContext : DbContext
    {
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Owner).HasMaxLength(200);
                b.Property(a => a.Balance).HasColumnType("decimal(18,2)");
            });
        }
    }
}
