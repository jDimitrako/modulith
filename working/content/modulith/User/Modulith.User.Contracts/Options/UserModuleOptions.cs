namespace Modulith.User.Contracts.Options;

public class UserModuleOptions
{
    public int PasswordHashIterations { get; set; } = 10000;
    public int RefreshTokenExpiryDays { get; set; } = 7;
    public bool RequireEmailConfirmation { get; set; } = true;
    public List<string> DefaultRoles { get; set; } = new() { "User" };
    public EmailSettings EmailSettings { get; set; } = new();
}

public class EmailSettings
{
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
} 