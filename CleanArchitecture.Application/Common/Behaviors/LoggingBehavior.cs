using MediatR;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Common.Behaviors;

/// <summary>Logs the request name and the calling user for every request, before it's handled.</summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling {RequestName} for user {UserId} ({UserName})",
            typeof(TRequest).Name,
            _currentUserService.UserId ?? "anonymous",
            _currentUserService.UserName ?? "anonymous");

        return next();
    }
}
