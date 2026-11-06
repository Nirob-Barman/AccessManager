using CleanArchitecture.Application.Common.Models.Authorization;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure.Authorization
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            string claimType,
            string claimValue);
        Task<List<string>> GetClaimsAsync(
            ClaimsPrincipal user,
            string claimType);
        Task<List<SubMenuClaimViewModel>> GetSubMenusAsync(ClaimsPrincipal user);
        Task<List<MenuItem>> GetUserMenusAsync(ClaimsPrincipal user);
        Task<List<string>> GetUserPermissionsAsync(ClaimsPrincipal user);
        Task<Dictionary<string, List<string>>> GetAllClaimsAsync(ClaimsPrincipal user);
    }
}