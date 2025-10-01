using Currency.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Currency.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Core.Entities.Currency> Currencies { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Core.Entities.Currency>(entity =>
        {
            entity.HasKey(_ => _.Id);
            entity.HasIndex(_ => _.NumCode).IsUnique();
            entity.HasIndex(_ => _.CharCode).IsUnique();
            entity.Property(_ => _.CbrId).HasMaxLength(20).IsRequired();
            entity.Property(_ => _.CharCode).HasMaxLength(10).IsRequired();
            entity.Property(_ => _.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(_ => _.Id);
            entity.HasIndex(e => new { e.CurrencyId, e.RateDate }).IsUnique();
            entity.HasOne(e => e.Currency)
                .WithMany(c => c.ExchangeRates)
                .HasForeignKey(e => e.CurrencyId);
        });
    }
}