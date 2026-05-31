using Microsoft.EntityFrameworkCore;
using MiniECommerce.Domain.Entities;
using MiniECommerce.Domain.Enums;

namespace MiniECommerce.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id)
                .HasName("pk_users");

            entity.Property(user => user.Id)
                .HasColumnName("id");

            entity.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(256)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique()
                .HasDatabaseName("ix_users_email");

            entity.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(user => user.Role)
                .HasColumnName("role")
                .HasConversion(
                    role => role.ToString(),
                    value => Enum.Parse<UserRole>(value))
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
        });
    }
}
