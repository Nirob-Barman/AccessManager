using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Models.GoogleLogin;
using CleanArchitecture.Application.Common.Models.Identity;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<IdentityOperationResult> CreateCustomerAsync(string email, string password, string fullName, string? phoneNumber);

    Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password);
    Task<AuthenticatedUser?> GetOrCreateGoogleUserAsync(GoogleUserInfo googleUser, CancellationToken cancellationToken = default);

    Task<UserProfile?> GetProfileAsync(string userId);

    Task<bool> UpdateProfileAsync(string userId, string fullName, string? phoneNumber, string? address, string? profileImageUrl);

    Task<int> GetCustomerCountAsync();

    Task<PagedResult<CustomerAccount>> GetCustomersAsync(string? search, int pageNumber, int pageSize);

    Task<CustomerAccount?> GetCustomerAccountByIdAsync(string userId);

    Task<bool> SetCustomerLockoutAsync(string userId, bool locked);
}
