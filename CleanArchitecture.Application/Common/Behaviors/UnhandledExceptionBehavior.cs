using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Common.Exceptions;

namespace CleanArchitecture.Application.Common.Behaviors;

/// <summary>
/// Logs exceptions that aren't one of the app's own expected/handled exception types (those are
/// already logged, with the right severity, by GlobalExceptionHandler) before rethrowing —
/// surfaces genuinely unexpected failures with the failing request's name attached.
/// </summary>
public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception) when (exception is not (ValidationException or NotFoundException or
            BadRequestException or IdentityException or UnauthorizedAccessException))
        {
            _logger.LogError(exception, "Unhandled exception for request {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}
