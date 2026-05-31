using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiniECommerce.Domain.Entities;
using MiniECommerce.Domain.Enums;

#nullable disable

namespace MiniECommerce.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.8")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id)
                .HasName("pk_users");

            entity.Property(user => user.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            entity.Property(user => user.Email)
                .HasColumnName("email")
                .HasColumnType("character varying(256)")
                .HasMaxLength(256)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique()
                .HasDatabaseName("ix_users_email");

            entity.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("character varying(512)")
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(user => user.Role)
                .HasColumnName("role")
                .HasColumnType("character varying(32)")
                .HasConversion(
                    role => role.ToString(),
                    value => Enum.Parse<UserRole>(value))
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        });
    }
}
