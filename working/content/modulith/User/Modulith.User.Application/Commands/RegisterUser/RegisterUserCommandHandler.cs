using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Modulith.User.Application.DomainEventHandlers;
using Modulith.User.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Modulith.User.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly UserDomainEventHandler _domainEventHandler;

    public RegisterUserCommandHandler(
        UserManager<User> _userManager,
        ILogger<RegisterUserCommandHandler> logger,
        UserDomainEventHandler domainEventHandler)
    {
        this._userManager = _userManager;
        _logger = logger;
        _domainEventHandler = domainEventHandler;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering new user with email {Email}", request.Email);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("User with email {Email} already exists", request.Email);
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        var user = User.Create(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create user: {Errors}", errors);
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        // Publish domain events
        await _domainEventHandler.HandleDomainEventsAsync(user);

        _logger.LogInformation("Successfully registered user {UserId}", user.Id);
        return user.Id;
    }
} 