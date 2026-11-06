using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Application.Features.RolePermissions.Dtos;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure.Authorization;

public class RolePermissionService : IRolePermissionService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public RolePermissionService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    public async Task<RolePermissionDto> GetRolePermissionsAsync(
        string? roleId,
        CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .Where(r =>
                r.Name != RoleConstants.SuperAdmin &&
                r.Name != RoleConstants.SystemAdmin)
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(
                r.Id,
                r.Name!))
            .ToListAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(roleId))
            roleId = roles.FirstOrDefault()?.Id;

        var assignedPermissions = new List<string>();

        if (!string.IsNullOrWhiteSpace(roleId))
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);

                assignedPermissions = claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        return new RolePermissionDto(
            roleId ?? string.Empty,
            roles,
            MenuConfiguration.GetMenus(),
            assignedPermissions);
    }

    public async Task<bool> SaveRolePermissionsAsync(
        string roleId,
        List<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
            return false;

        // Never modify protected system roles.
        if (role.Name == RoleConstants.SuperAdmin ||
            role.Name == RoleConstants.SystemAdmin)
        {
            return false;
        }

        // Validate submitted permissions against the configured catalog.
        var allowedPermissions = MenuConfiguration.GetMenus()
            .SelectMany(menu => menu.SubMenus.SelectMany(submenu =>
                submenu.Permissions.Select(permission =>
                    $"{menu.Key}.{submenu.Key}.{permission}")))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectedPermissions = (permissions ?? new List<string>())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (selectedPermissions.Any(p => !allowedPermissions.Contains(p)))
            return false;

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in existingClaims
                .Where(c => c.Type == "Permission").ToList())
            {
                var removeResult = await _roleManager.RemoveClaimAsync(
                    role, claim);

                if (!removeResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }
            }

            foreach (var permission in selectedPermissions)
            {
                var addResult = await _roleManager.AddClaimAsync(
                    role,
                    new Claim("Permission", permission));

                if (!addResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }
            }

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}