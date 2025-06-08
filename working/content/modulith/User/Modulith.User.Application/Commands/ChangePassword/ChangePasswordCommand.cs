using MediatR;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Entities;

namespace Modulith.User.Application.Commands.ChangePassword;

public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<ChangePasswordResponse>;

public record ChangePasswordResponse(bool Succeeded, string? ErrorMessage);

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        UserManager<User> userManager,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new ChangePasswordResponse(false, "User not found");
        }

        if (!user.IsActive)
        {
            return new ChangePasswordResponse(false, "Account is deactivated");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to change password for user {UserId}: {Errors}", request.UserId, errors);
            return new ChangePasswordResponse(false, errors);
        }

        return new ChangePasswordResponse(true, null);
    }
} 