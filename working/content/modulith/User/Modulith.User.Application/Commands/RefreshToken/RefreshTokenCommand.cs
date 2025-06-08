using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modulith.User.Domain.Entities;
using Modulith.User.Infrastructure.Persistence;
using Modulith.User.Infrastructure.Services;

namespace Modulith.User.Application.Commands.RefreshToken;

public record RefreshTokenCommand(
    string Token,
    string RefreshToken) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(
    string Token,
    string NewRefreshToken,
    DateTime ExpiresAt);

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UserDbContext _dbContext;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        UserManager<User> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        UserDbContext dbContext,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Get the refresh token
        var refreshToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        if (!refreshToken.IsActive)
        {
            throw new InvalidOperationException("Refresh token is no longer active");
        }

        // Get the user
        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("User account is deactivated");
        }

        // Generate new tokens
        var newToken = _jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Revoke the old refresh token
        refreshToken.Revoke(newRefreshToken, "Replaced by new refresh token");

        // Save the new refresh token
        var newRefreshTokenEntity = RefreshToken.Create(
            newRefreshToken,
            user.Id,
            DateTime.UtcNow.AddDays(7));

        _dbContext.RefreshTokens.Add(newRefreshTokenEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(
            Token: newToken,
            NewRefreshToken: newRefreshToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(60)
        );
    }
} 