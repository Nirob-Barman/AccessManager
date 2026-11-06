
using MediatR;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _identityService.ValidateCredentialsAsync(
            request.Email,
            request.Password);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return new AuthResponseDto(user.UserId, user.Email, user.FullName, user.Role);
    }
}
