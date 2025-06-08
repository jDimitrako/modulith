namespace Modulith.User.Contracts.Authorization;

public static class UserPolicies
{
    public const string RequireAdminRole = "RequireAdminRole";
    public const string RequireUserRole = "RequireUserRole";
    public const string RequireActiveUser = "RequireActiveUser";
    public const string RequireVerifiedEmail = "RequireVerifiedEmail";
} 