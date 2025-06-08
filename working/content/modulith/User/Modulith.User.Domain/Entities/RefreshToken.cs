using System.ComponentModel.DataAnnotations;

namespace Modulith.User.Domain.Entities;

public class RefreshToken
{
    [Key]
    public string Token { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? ReasonRevoked { get; private set; }

    private RefreshToken() { } // For EF Core

    public static RefreshToken Create(string token, string userId, DateTime expiryDate)
    {
        return new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiryDate = expiryDate,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke(string? replacedByToken = null, string? reason = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
        ReasonRevoked = reason;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public bool IsActive => !IsRevoked && !IsExpired;
} 