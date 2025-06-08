using MediatR;
using Microsoft.Extensions.Logging;

namespace Modulith.NewModule.Api;

public class MediatRLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<MediatRLoggingBehavior<TRequest, TResponse>> _logger;
    public MediatRLoggingBehavior(ILogger<MediatRLoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
} 