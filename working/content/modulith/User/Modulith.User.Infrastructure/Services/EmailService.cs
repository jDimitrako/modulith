using System.Net.Mail;
using Microsoft.Extensions.Options;
using Modulith.User.Contracts.Options;

namespace Modulith.User.Infrastructure.Services;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink);
    Task SendPasswordResetAsync(string email, string resetLink);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<UserModuleOptions> options, ILogger<EmailService> logger)
    {
        _emailSettings = options.Value.EmailSettings;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var subject = "Confirm your email";
        var body = $@"
            <h2>Welcome to Modulith!</h2>
            <p>Please confirm your email address by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Confirm Email</a></p>
            <p>If you did not create an account, you can safely ignore this email.</p>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "Reset your password";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>You have requested to reset your password. Click the link below to proceed:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you did not request a password reset, you can safely ignore this email.</p>";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.UseSsl,
                Credentials = new System.Net.NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", to);
            throw;
        }
    }
} 