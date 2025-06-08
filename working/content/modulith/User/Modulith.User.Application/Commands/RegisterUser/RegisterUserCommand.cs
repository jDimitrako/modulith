using MediatR;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Entities;

namespace Modulith.User.Application.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<IdentityResult>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private readonly UserManager<User> _userManager;

    public RegisterUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(
            request.Email,
            request.FirstName,
            request.LastName);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // Add user to default role
            await _userManager.AddToRoleAsync(user, "User");
        }

        return result;
    }
} 