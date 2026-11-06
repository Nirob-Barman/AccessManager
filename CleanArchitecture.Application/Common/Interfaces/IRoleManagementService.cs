using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IRoleManagementService
{
    Task<List<RoleManagementDto>> GetRolesAsync();

    Task<RoleDetailsDto?> GetRoleByIdAsync(string id);

    Task<AssignUsersDto?> GetUsersForRoleAsync(string roleId);

    Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(
        string name);

    Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(
        string id,
        string name);

    Task<(bool Succeeded, string Error)> DeleteRoleAsync(
        string id);

    Task<(bool Succeeded, string Error)> AssignUsersAsync(
        string roleId,
        List<string> selectedUserIds);
}