using MediatR;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Entities;
using Modulith.User.Infrastructure.Persistence;
using Modulith.User.Infrastructure.Services;

namespace Modulith.User.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    bool RememberMe = false) : IRequest<LoginResponse>;

public record LoginResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfoResponse User);

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UserDbContext _dbContext;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        UserDbContext dbContext,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Account is deactivated");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        // Generate JWT token
        var token = _jwtTokenGenerator.GenerateToken(user, request.RememberMe);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Save refresh token
        var expiryDate = DateTime.UtcNow.AddDays(7); // 7 days expiry for refresh token
        var refreshTokenEntity = RefreshToken.Create(refreshToken, user.Id, expiryDate);
        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Update last login
        user.UpdateLastLogin();
        await _userManager.UpdateAsync(user);

        return new LoginResponse(
            Token: token,
            RefreshToken: refreshToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(request.RememberMe ? 10080 : 60), // 7 days or 1 hour
            User: new UserInfoResponse(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Roles: user.Roles,
                IsActive: user.IsActive,
                EmailConfirmed: await _userManager.IsEmailConfirmedAsync(user),
                ExternalLogins: (await _userManager.GetLoginsAsync(user)).Select(l => l.LoginProvider).ToList()
            )
        );
    }
} 