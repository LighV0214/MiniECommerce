using MiniECommerce.Domain.Enums;

namespace MiniECommerce.Domain.Entities;

public class User
{
    private User()
    {
    }

    public User(string email, string passwordHash, UserRole role = UserRole.Customer)
    {
        Id = Guid.NewGuid();
        Email = NormalizeEmail(email);
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; } = UserRole.Customer;
    public DateTime CreatedAt { get; private set; }

    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
