using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Contracts.Authorization;
using Modulith.User.Domain.Entities;

namespace Modulith.User.Infrastructure.Authorization;

public class ActiveUserHandler : AuthorizationHandler<ActiveUserRequirement>
{
    private readonly UserManager<User> _userManager;

    public ActiveUserHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ActiveUserRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);
        if (user == null)
        {
            return;
        }

        if (user.IsActive)
        {
            context.Succeed(requirement);
        }
    }
} 