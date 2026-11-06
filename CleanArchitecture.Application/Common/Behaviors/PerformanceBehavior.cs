using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Common.Behaviors;

/// <summary>Logs a warning for any request that takes longer than 500ms to handle.</summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "Long running request: {RequestName} ({ElapsedMilliseconds}ms) for user {UserId}",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                _currentUserService.UserId ?? "anonymous");
        }

        return response;
    }
}
