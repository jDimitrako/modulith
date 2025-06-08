using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modulith.Template.Domain.Interfaces;

namespace Modulith.Template.Api.Workers;

public class TemplateWorker : BackgroundService
{
    private readonly ILogger<TemplateWorker> _logger;
    private readonly IUserRepository _userRepository;

    public TemplateWorker(
        ILogger<TemplateWorker> logger,
        IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Template Worker running at: {time}", DateTimeOffset.Now);
                
                // Add any module-specific background tasks here
                // For example, you might want to:
                // - Clean up old data
                // - Send periodic notifications
                // - Update cached data
                // - etc.

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing Template Worker");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait a bit before retrying
            }
        }
    }
} 