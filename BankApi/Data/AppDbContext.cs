using BankApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<TransactionLog> TransactionLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique();
                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasMany(c => c.PhoneNumbers)
                    .WithOne(c => c.Client)
                    .HasForeignKey(c => c.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.BankAccounts)
                    .WithOne(b => b.Client)
                    .HasForeignKey(b => b.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasIndex(b => b.AccountNumber).IsUnique();


            entity.Property(b => b.Balance)
                .HasPrecision(18, 2);

            entity.HasMany(b => b.Cards)
                .WithOne(c => c.BankAccount)
                .HasForeignKey(c => c.BankAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasIndex(c => c.CardNumber).IsUnique();
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.Property(t => t.FromAccountNumber).HasMaxLength(64);
            entity.Property(t => t.ToAccountNumber).HasMaxLength(64);
            entity.Property(t => t.Currency).HasMaxLength(8);
        });
    }
}
