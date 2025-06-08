using Microsoft.AspNetCore.Authorization;

namespace Modulith.User.Contracts.Authorization;

public class ActiveUserRequirement : IAuthorizationRequirement
{
}

public class VerifiedEmailRequirement : IAuthorizationRequirement
{
} 