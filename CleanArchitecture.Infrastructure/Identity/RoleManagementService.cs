using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;
namespace CleanArchitecture.Infrastructure.Identity;

public class RoleManagementService : IRoleManagementService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly HashSet<string> ProtectedRoles =
        new(StringComparer.OrdinalIgnoreCase)
        {
            RoleConstants.SuperAdmin,
            RoleConstants.SystemAdmin
        };

    public RoleManagementService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    private static bool IsProtectedRole(string? roleName)
    {
        return !string.IsNullOrWhiteSpace(roleName)
               && ProtectedRoles.Contains(roleName);
    }

    // GET: All Roles
    public async Task<List<RoleManagementDto>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .ToListAsync();

        var result = new List<RoleManagementDto>();

        foreach (var role in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(
                role.Name!);

            result.Add(new RoleManagementDto(
                role.Id,
                role.Name!,
                users.Count
            ));
        }

        return result;
    }

    // GET: Role By Id
    public async Task<RoleDetailsDto?> GetRoleByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return null;

        if (IsProtectedRole(role.Name))
            return null;

        return new RoleDetailsDto(
            role.Id,
            role.Name!
        );
    }

    // GET: Users For Role
    public async Task<AssignUsersDto?> GetUsersForRoleAsync(
        string roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            return null;

        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
            return null;

        if (IsProtectedRole(role.Name))
            return null;

        // Get users belonging to protected roles.
        var superAdmins = await _userManager.GetUsersInRoleAsync(
            RoleConstants.SuperAdmin);

        var systemAdmins = await _userManager.GetUsersInRoleAsync(
            RoleConstants.SystemAdmin);

        // Exclude protected-role users.
        var excludedUserIds = superAdmins
            .Select(u => u.Id)
            .Concat(systemAdmins.Select(u => u.Id))
            .ToHashSet();

        var users = await _userManager.Users
            .Where(u => !excludedUserIds.Contains(u.Id))
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var assignedUsers = await _userManager.GetUsersInRoleAsync(
            role.Name!);

        var assignedIds = assignedUsers
            .Select(u => u.Id)
            .ToHashSet();

        var userDtos = users.Select(user =>
            new UserRoleDto(
                user.Id,
                user.UserName ?? "",
                user.Email ?? "",
                assignedIds.Contains(user.Id)
            )).ToList();

        return new AssignUsersDto(
            role.Id,
            role.Name!,
            userDtos
        );
    }

    // POST: Create Role
    public async Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, new[] { "Role name is required." });

        var roleName = name.Trim();

        if (IsProtectedRole(roleName))
        {
            return (false, new[]
            {
                "This is a reserved system role."
            });
        }

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return (false, new[]
            {
                "This role already exists."
            });
        }

        var result = await _roleManager.CreateAsync(
            new IdentityRole(roleName));

        if (!result.Succeeded)
        {
            return (
                false,
                result.Errors.Select(e => e.Description).ToArray()
            );
        }

        return (true, Array.Empty<string>());
    }

    // POST: Update Role
    public async Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(
        string id,
        string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            return (false, new[] { "Invalid role ID." });

        if (string.IsNullOrWhiteSpace(name))
            return (false, new[] { "Role name is required." });

        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return (false, new[] { "Role not found." });

        if (IsProtectedRole(role.Name))
            return (false, new[] { "This role cannot be edited." });

        var newName = name.Trim();

        if (IsProtectedRole(newName))
        {
            return (false, new[]
            {
                "This is a reserved system role."
            });
        }

        var existingRole = await _roleManager.FindByNameAsync(
            newName);

        if (existingRole != null && existingRole.Id != role.Id)
        {
            return (false, new[]
            {
                "This role name already exists."
            });
        }

        role.Name = newName;

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return (
                false,
                result.Errors.Select(e => e.Description).ToArray()
            );
        }

        return (true, Array.Empty<string>());
    }

    // POST: Delete Role
    public async Task<(bool Succeeded, string Error)> DeleteRoleAsync(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return (false, "Invalid role ID.");

        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return (false, "Role not found.");

        if (IsProtectedRole(role.Name))
            return (false, "This role cannot be deleted.");

        // Do not delete roles that have assigned users.
        var users = await _userManager.GetUsersInRoleAsync(
            role.Name!);

        if (users.Count > 0)
        {
            return (
                false,
                "Cannot delete this role because users are assigned to it."
            );
        }

        // Do not delete roles that still have permission claims.
        var claims = await _roleManager.GetClaimsAsync(role);

        if (claims.Count > 0)
        {
            return (
                false,
                "Remove the role's assigned permissions before deleting it."
            );
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return (false, "Failed to delete role.");
        }

        return (true, string.Empty);
    }

    // POST: Assign Users
    public async Task<(bool Succeeded, string Error)> AssignUsersAsync(
        string roleId,
        List<string> selectedUserIds)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            return (false, "Invalid role ID.");

        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
            return (false, "Role not found.");

        if (IsProtectedRole(role.Name))
            return (false, "Users cannot be assigned to this role.");

        selectedUserIds ??= new List<string>();

        var selectedIds = selectedUserIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet();

        var allUsers = await _userManager.Users.ToListAsync();

        var allUserIds = allUsers
            .Select(u => u.Id)
            .ToHashSet();

        // Validate submitted user IDs.
        if (selectedIds.Except(allUserIds).Any())
        {
            return (false, "Invalid user selection.");
        }

        // Prevent assigning users belonging to protected roles.
        var superAdmins = await _userManager.GetUsersInRoleAsync(
            RoleConstants.SuperAdmin);

        var systemAdmins = await _userManager.GetUsersInRoleAsync(
            RoleConstants.SystemAdmin);

        var protectedUserIds = superAdmins
            .Select(u => u.Id)
            .Concat(systemAdmins.Select(u => u.Id))
            .ToHashSet();

        if (selectedIds.Overlaps(protectedUserIds))
        {
            return (
                false,
                "Users belonging to protected roles cannot be assigned."
            );
        }

        // Get currently assigned users.
        var currentUsers = await _userManager.GetUsersInRoleAsync(
            role.Name!);

        var currentIds = currentUsers
            .Select(u => u.Id)
            .ToHashSet();

        // Remove users who are no longer selected.
        foreach (var user in currentUsers)
        {
            if (!selectedIds.Contains(user.Id))
            {
                var result = await _userManager.RemoveFromRoleAsync(
                    user,
                    role.Name!);

                if (!result.Succeeded)
                {
                    return (
                        false,
                        "Failed to remove a user from the role."
                    );
                }
            }
        }

        // Add newly selected users.
        foreach (var user in allUsers)
        {
            if (selectedIds.Contains(user.Id)
                && !currentIds.Contains(user.Id))
            {
                var result = await _userManager.AddToRoleAsync(
                    user,
                    role.Name!);

                if (!result.Succeeded)
                {
                    return (
                        false,
                        "Failed to assign a user to the role."
                    );
                }
            }
        }

        return (true, string.Empty);
    }
}
