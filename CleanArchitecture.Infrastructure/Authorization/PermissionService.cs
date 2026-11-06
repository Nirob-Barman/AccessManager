using CleanArchitecture.Application.Common.Models.Authorization;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure.Authorization
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            string claimType,
            string claimValue)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return false;

            //var hasPermission = user.Claims.Any(c => c.Type == claimType && c.Value == claimValue);

            var appUser = await _userManager.GetUserAsync(user);

            if (appUser == null)
                return false;

            var roles = await _userManager.GetRolesAsync(appUser);

            // Super Admin bypass
            if (roles.Contains(RoleConstants.SuperAdmin, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            // Check individual user claims first
            //var userClaims = await _userManager.GetClaimsAsync(appUser);

            //if (userClaims.Any(
            //    c =>c.Type == claimType &&
            //    c.Value == claimValue))
            //{
            //    return true;
            //}

            // Get user's assigned roles
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                    continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);

                if (roleClaims.Any(c =>
                    c.Type == claimType &&
                    c.Value == claimValue))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<List<string>> GetClaimsAsync(
            ClaimsPrincipal user,
            string claimType)
        {
            if(user == null)
                return new List<string>();

            if(user.Identity?.IsAuthenticated != true)
                return new List<string>();

            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //if (user?.Identity?.IsAuthenticated != true)
            //    return result.ToList();

            var appUser = await _userManager.GetUserAsync(user);

            if (appUser == null)
                return result.ToList();

            // User claims
            var userClaims = await _userManager.GetClaimsAsync(appUser);

            foreach (var claim in userClaims.Where(c => c.Type == claimType))
            {
                result.Add(claim.Value);
            }

            // Role claims
            var roles = await _userManager.GetRolesAsync(appUser);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                    continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in roleClaims.Where(c => c.Type == claimType))
                {
                    result.Add(claim.Value);
                }
            }

            return result.ToList();
        }

        public async Task<Dictionary<string, List<string>>> GetAllClaimsAsync(ClaimsPrincipal user)
        {
            var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (user?.Identity?.IsAuthenticated != true)
                return result;

            var appUser = await _userManager.GetUserAsync(user);

            if (appUser == null)
                return result;

            var userClaims = await _userManager.GetClaimsAsync(appUser);

            var roles = await _userManager.GetRolesAsync(appUser);

            //if (roles.Contains(RoleConstants.SuperAdmin, StringComparer.OrdinalIgnoreCase))
            //{
            //    var allPermissions = _menuConfiguration.GetMenus()
            //        .SelectMany(menu => menu.SubMenus.SelectMany(submenu =>
            //            submenu.Permissions.Select(permission =>
            //                $"{menu.Key}.{submenu.Key}.{permission}")))
            //        .Distinct(StringComparer.OrdinalIgnoreCase)
            //        .ToList();
            //    result["Permission"] = allPermissions;
            //}

            var allClaims = new List<Claim>(userClaims);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                    continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);

                allClaims.AddRange(roleClaims);
            }

            result = allClaims
                .GroupBy(c => c.Type, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => c.Value)
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          .ToList(),
                    StringComparer.OrdinalIgnoreCase);

            return result;
        }


        public async Task<List<MenuItem>> GetUserMenusAsync(ClaimsPrincipal user)
        {
            var permissions = await GetUserPermissionsAsync(user);

            var configuredMenus = MenuConfiguration.GetMenus();

            var result = new List<MenuItem>();

            foreach (var menu in configuredMenus)
            {
                var allowedSubMenus = menu.SubMenus
                    .Where(submenu =>
                        permissions.Contains(
                            $"{menu.Key}.{submenu.Key}.View",
                            StringComparer.OrdinalIgnoreCase))
                    .OrderBy(s => s.SortOrder)
                    .ToList();

                if (allowedSubMenus.Any())
                {
                    result.Add(new MenuItem
                    {
                        Key = menu.Key,
                        Name = menu.Name,
                        Icon = menu.Icon,
                        SortOrder = menu.SortOrder,
                        SubMenus = allowedSubMenus
                    });
                }
            }

            return result.OrderBy(m => m.SortOrder).ToList();
        }


        public async Task<List<SubMenuClaimViewModel>> GetSubMenusAsync(ClaimsPrincipal user)
        {
            var claimValues = await GetClaimsAsync(user, "SubMenu");

            var subMenus = new List<SubMenuClaimViewModel>();

            foreach (var value in claimValues)
            {
                try
                {
                    var submenu =
                        System.Text.Json.JsonSerializer
                            .Deserialize<SubMenuClaimViewModel>(value);

                    if (submenu != null)
                        subMenus.Add(submenu);
                }
                catch (System.Text.Json.JsonException)
                {
                    // Ignore old submenu claims stored as simple strings.
                }
            }

            return subMenus
                .OrderBy(s => s.SortOrder)
                .ToList();
        }

        public async Task<List<string>> GetUserPermissionsAsync(ClaimsPrincipal user)
        {
            Console.WriteLine("GetUserPermissionsAsync CALLED");

            var permissions = new List<string>();

            if (user?.Identity?.IsAuthenticated != true)
                return permissions;

            var userId = _userManager.GetUserId(user);

            if (string.IsNullOrEmpty(userId))
                return permissions;

            
            //var applicationUser = await _userManager.FindByIdAsync(userId);
            var applicationUser = await _userManager.GetUserAsync(user);

            if (applicationUser == null)
                return permissions;

            var roles = await _userManager.GetRolesAsync(applicationUser);
            
            // Super Admin gets every configured permission
            if (roles.Contains(RoleConstants.SuperAdmin, StringComparer.OrdinalIgnoreCase))
            {
                permissions =  MenuConfiguration.GetMenus()
                    .SelectMany(menu => menu.SubMenus.SelectMany(submenu =>
                        submenu.Permissions.Select(permission =>
                            $"{menu.Key}.{submenu.Key}.{permission}")))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                
                return permissions;
            }

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                    continue;

                var claims = await _roleManager.GetClaimsAsync(role);

                permissions.AddRange(
                    claims
                        .Where(c => c.Type == "Permission")
                        .Select(c => c.Value)
                );
            }

            permissions = permissions.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            return permissions;
        }
    }
}