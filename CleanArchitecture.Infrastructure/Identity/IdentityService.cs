using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Models.GoogleLogin;
using CleanArchitecture.Application.Common.Models.Identity;
using CleanArchitecture.Domain.Constants;


namespace CleanArchitecture.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityOperationResult> CreateCustomerAsync(string email, string password, string fullName, string? phoneNumber)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return new IdentityOperationResult(false, null, new[] { "An account with this email already exists." });
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return new IdentityOperationResult(false, null, result.Errors.Select(e => e.Description).ToArray());
        }

        await _userManager.AddToRoleAsync(user, Roles.Customer);

        return new IdentityOperationResult(true, user.Id, Array.Empty<string>());
    }

    public async Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? Roles.Customer;

        return new AuthenticatedUser(user.Id, user.Email!, user.FullName, role);
    }

    public async Task<AuthenticatedUser?> GetOrCreateGoogleUserAsync(GoogleUserInfo googleUser, CancellationToken cancellationToken = default)
    {
        const string loginProvider = "Google";
        var user = await _userManager.FindByLoginAsync(loginProvider, googleUser.GoogleId);

        if (user is not null)
        {
            return await ToAuthenticatedUserAsync(user);
        }

        user = await _userManager.FindByEmailAsync(googleUser.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = googleUser.Email,
                Email = googleUser.Email,
                FullName = googleUser.FullName ?? googleUser.Email,
                EmailConfirmed = true,
                ProfileImageUrl = googleUser.Picture
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return null;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);

            if (!roleResult.Succeeded)
            {
                return null;
            }
        }
        //else
        //{
        //    // Keep the existing user's profile image updated from Google
        //    // only when Google provides one.
        //    if (!string.IsNullOrWhiteSpace(googleUser.Picture) && user.ProfileImageUrl != googleUser.Picture)
        //    {
        //        user.ProfileImageUrl = googleUser.Picture;

        //        var updateResult = await _userManager.UpdateAsync(user);

        //        if (!updateResult.Succeeded)
        //        {
        //            return null;
        //        }
        //    }
        //}

        var loginInfo = new UserLoginInfo(loginProvider, googleUser.GoogleId, loginProvider);
        //Link the Google account using AddLoginAsync.
        var loginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!loginResult.Succeeded)
        {
            return null;
        }

        return await ToAuthenticatedUserAsync(user);
    }


    public async Task<UserProfile?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? Roles.Customer;

        return new UserProfile(user.Id, user.Email!, user.FullName, user.PhoneNumber, user.Address, user.ProfileImageUrl, role);
    }

    public async Task<bool> UpdateProfileAsync(string userId, string fullName, string? phoneNumber, string? address, string? profileImageUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        user.FullName = fullName;
        user.PhoneNumber = phoneNumber;
        user.Address = address;
        user.ProfileImageUrl = profileImageUrl;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<int> GetCustomerCountAsync()
    {
        var customers = await _userManager.GetUsersInRoleAsync(Roles.Customer);
        return customers.Count;
    }

    public async Task<PagedResult<CustomerAccount>> GetCustomersAsync(string? search, int pageNumber, int pageSize)
    {
        var customers = await _userManager.GetUsersInRoleAsync(Roles.Customer);

        var filtered = customers
            .Where(u => string.IsNullOrWhiteSpace(search)
                || (u.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                || u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
            .OrderBy(u => u.FullName)
            .ToList();

        var page = filtered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var accounts = new List<CustomerAccount>(page.Count);
        foreach (var user in page)
        {
            accounts.Add(await ToCustomerAccountAsync(user));
        }

        return new PagedResult<CustomerAccount>(accounts, filtered.Count, pageNumber, pageSize);
    }

    public async Task<CustomerAccount?> GetCustomerAccountByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !await _userManager.IsInRoleAsync(user, Roles.Customer))
        {
            return null;
        }

        return await ToCustomerAccountAsync(user);
    }

    public async Task<bool> SetCustomerLockoutAsync(string userId, bool locked)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !await _userManager.IsInRoleAsync(user, Roles.Customer))
        {
            return false;
        }

        await _userManager.SetLockoutEnabledAsync(user, true);
        var result = await _userManager.SetLockoutEndDateAsync(user, locked ? DateTimeOffset.MaxValue : null);
        return result.Succeeded;
    }

    private async Task<CustomerAccount> ToCustomerAccountAsync(ApplicationUser user)
    {
        var isLockedOut = await _userManager.IsLockedOutAsync(user);
        return new CustomerAccount(user.Id, user.Email!, user.FullName, user.PhoneNumber, user.Address, isLockedOut);
    }

    private async Task<AuthenticatedUser> ToAuthenticatedUserAsync(ApplicationUser user)
    {
        var role = (await _userManager.GetRolesAsync(user)) .FirstOrDefault() ?? Roles.Customer;

        return new AuthenticatedUser(
            user.Id,
            user.Email!,
            user.FullName,
            role);
    }
}
