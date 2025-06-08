using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Modulith.NewModule.Domain.Entities;
using Modulith.NewModule.Domain.Interfaces;
using Modulith.User.Contracts.Events;
using System.Threading.Tasks;

namespace Modulith.NewModule.Application.IntegrationEvents;

public class UserCreatedEventHandler : ICapSubscribe
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(
        IUserRepository userRepository,
        ILogger<UserCreatedEventHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [CapSubscribe("UserCreated")]
    public async Task HandleAsync(UserCreatedEvent @event)
    {
        _logger.LogInformation("Handling UserCreated event for user {UserId}", @event.UserId);

        if (await _userRepository.ExistsAsync(@event.UserId))
        {
            _logger.LogWarning("User {UserId} already exists in new module", @event.UserId);
            return;
        }

        var user = User.Create(@event.UserId);
        await _userRepository.AddAsync(user);

        _logger.LogInformation("Successfully created user {UserId} in new module", @event.UserId);
    }
} 