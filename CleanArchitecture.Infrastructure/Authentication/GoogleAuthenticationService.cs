using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models.GoogleLogin;

namespace CleanArchitecture.Infrastructure.Authentication;

public sealed class GoogleAuthenticationService : IGoogleAuthenticationService
{
    private readonly string _clientId;

    public GoogleAuthenticationService(IConfiguration configuration)
    {
        _clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException(
                "Google authentication ClientId is not configured.");
    }

    public async Task<GoogleUserInfo> ValidateTokenAsync(string credential, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(credential))
        {
            throw new ArgumentException("Google credential is required.", nameof(credential));
        }

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            });

        if (string.IsNullOrWhiteSpace(payload.Subject))
        {
            throw new UnauthorizedAccessException("Google token does not contain a subject.");
        }

        if (string.IsNullOrWhiteSpace(payload.Email))
        {
            throw new UnauthorizedAccessException("Google account does not contain an email.");
        }

        return new GoogleUserInfo(
            payload.Subject,
            payload.Email,
            payload.Name,
            payload.Picture);
    }
}