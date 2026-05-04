using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PTPI.Models;

namespace PTPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Person>(entity =>
            {
                entity.HasIndex(e => e.IdNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Person_id");
            });

            builder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.AccountNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Account_num");

                entity.HasOne(a => a.Person)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(a => a.PersonCode)
                    .HasConstraintName("FK_Account_Person")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.Account)
                    .WithMany(a => a.Transactions)
                    .HasForeignKey(t => t.AccountCode)
                    .HasConstraintName("FK_Transaction_Account")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
