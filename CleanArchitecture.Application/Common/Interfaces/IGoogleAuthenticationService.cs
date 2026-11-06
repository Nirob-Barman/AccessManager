using CleanArchitecture.Application.Common.Models.GoogleLogin;

namespace CleanArchitecture.Application.Common.Interfaces;
public interface IGoogleAuthenticationService
{
    Task<GoogleUserInfo> ValidateTokenAsync(string credential, CancellationToken cancellationToken = default);
}
