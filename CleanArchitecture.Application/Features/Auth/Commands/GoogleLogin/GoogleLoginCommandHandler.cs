using MediatR;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.GoogleLogin;

public sealed class GoogleLoginCommandHandler
    : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IGoogleAuthenticationService _googleAuthenticationService;

    public GoogleLoginCommandHandler(
        IIdentityService identityService,
        IGoogleAuthenticationService googleAuthenticationService)
    {
        _identityService = identityService;
        _googleAuthenticationService = googleAuthenticationService;
    }

    public async Task<AuthResponseDto> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await _googleAuthenticationService.ValidateTokenAsync(request.Credential, cancellationToken);

        if(googleUser is null)
        {
            throw new UnauthorizedAccessException("Invalid Google token.");
        }

        var user = await _identityService.GetOrCreateGoogleUserAsync(googleUser, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Google login failed.");
        }

        return new AuthResponseDto(
            user.UserId,
            user.Email,
            user.FullName,
            user.Role
        );
    }
}
