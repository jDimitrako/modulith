using MediatR;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Entities;

namespace Modulith.User.Application.Commands.ExternalLogin;

public record ExternalLoginCommand(
    string Provider,
    string ProviderKey,
    string Email,
    string FirstName,
    string LastName) : IRequest<ExternalLoginResult>;

public record ExternalLoginResult(
    bool Succeeded,
    string? Token,
    string? RefreshToken,
    string? ErrorMessage);

public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ExternalLoginResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public ExternalLoginCommandHandler(
        UserManager<User> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ExternalLoginResult> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Create new user
            user = User.Create(
                request.Email,
                request.FirstName,
                request.LastName);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return new ExternalLoginResult(false, null, null, "Failed to create user");
            }

            // Add to default role
            await _userManager.AddToRoleAsync(user, "User");
        }

        // Check if external login exists
        var externalLogin = await _userManager.FindByLoginAsync(request.Provider, request.ProviderKey);
        if (externalLogin == null)
        {
            // Add external login
            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo(
                request.Provider,
                request.ProviderKey,
                request.Provider));

            if (!result.Succeeded)
            {
                return new ExternalLoginResult(false, null, null, "Failed to add external login");
            }
        }

        // Update last login
        user.UpdateLastLogin();
        await _userManager.UpdateAsync(user);

        // Generate tokens
        var token = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Store refresh token
        await _userManager.SetAuthenticationTokenAsync(
            user,
            "Modulith",
            "RefreshToken",
            refreshToken);

        return new ExternalLoginResult(true, token, refreshToken, null);
    }
} 