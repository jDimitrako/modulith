using System.ComponentModel.DataAnnotations;

namespace Modulith.User.Contracts.DTOs;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);

public record LoginRequest(
    string Email,
    string Password,
    bool RememberMe = false);

public record AuthResponse(
    string Token,
    string RefreshToken);

public record UserInfoResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles,
    bool IsActive,
    bool EmailConfirmed,
    List<string> ExternalLogins);

public record RefreshTokenRequest(
    string Token,
    string RefreshToken);

public record ConfirmEmailRequest(
    string UserId,
    string Token);

public record ResetPasswordRequest(
    string Email);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword); 