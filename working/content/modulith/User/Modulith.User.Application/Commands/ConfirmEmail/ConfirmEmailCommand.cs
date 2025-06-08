using MediatR;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Entities;

namespace Modulith.User.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(
    string UserId,
    string Token) : IRequest<ConfirmEmailResponse>;

public record ConfirmEmailResponse(bool Succeeded, string? ErrorMessage);

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;

    public ConfirmEmailCommandHandler(
        UserManager<User> userManager,
        ILogger<ConfirmEmailCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ConfirmEmailResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new ConfirmEmailResponse(false, "User not found");
        }

        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            return new ConfirmEmailResponse(true, null);
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to confirm email for user {UserId}: {Errors}", request.UserId, errors);
            return new ConfirmEmailResponse(false, errors);
        }

        return new ConfirmEmailResponse(true, null);
    }
} 