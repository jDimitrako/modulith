using MediatR;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Entities;
using Modulith.User.Infrastructure.Services;

namespace Modulith.User.Application.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email) : IRequest<ResetPasswordResponse>;

public record ResetPasswordResponse(bool Succeeded, string? ErrorMessage);

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        UserManager<User> userManager,
        IEmailService emailService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            return new ResetPasswordResponse(true, null);
        }

        if (!user.IsActive)
        {
            return new ResetPasswordResponse(false, "Account is deactivated");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://your-app.com/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        try
        {
            await _emailService.SendPasswordResetAsync(user.Email, resetLink);
            return new ResetPasswordResponse(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            return new ResetPasswordResponse(false, "Failed to send password reset email");
        }
    }
} 