using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modulith.User.Domain.Entities;

namespace Modulith.User.Infrastructure.Persistence;

public class UserDbContext : IdentityDbContext<User>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FirstName).IsRequired();
            entity.Property(u => u.LastName).IsRequired();
            entity.Property(u => u.Roles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(rt => rt.Token);
            entity.Property(rt => rt.UserId).IsRequired();
            entity.Property(rt => rt.ExpiryDate).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 