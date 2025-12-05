using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Entities;

namespace PaymentsService.Infrastructure
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Payment>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                b.HasIndex(p => p.AccountId);
                b.Property(p => p.Currency).HasMaxLength(8);
                b.Property(p => p.Status).HasMaxLength(32);
            });
        }
    }
}
