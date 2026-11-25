using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<Character> Characters => Set<Character>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(account => account.Id);
            entity.Property(account => account.Username).HasMaxLength(50).IsRequired();
            entity.HasIndex(account => account.Username).IsUnique();
            entity.Property(account => account.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(account => account.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.ToTable("Characters");
            entity.HasKey(character => character.Id);
            entity.Property(character => character.Name).HasMaxLength(50).IsRequired();
            entity.Property(character => character.Class).HasMaxLength(30).IsRequired();
            entity.Property(character => character.Level).HasDefaultValue(1);
            entity.Property(character => character.Hp).HasDefaultValue(100);
            entity.Property(character => character.CreatedAt).IsRequired();

            entity.HasOne(character => character.Account)
                .WithMany(account => account.Characters)
                .HasForeignKey(character => character.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
