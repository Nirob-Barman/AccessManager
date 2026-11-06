using CleanArchitecture.Application.Features.RolePermissions.Dtos;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IRolePermissionService
{
    Task<RolePermissionDto> GetRolePermissionsAsync(
        string? roleId,
        CancellationToken cancellationToken = default);

    Task<bool> SaveRolePermissionsAsync(
        string roleId,
        List<string> permissions,
        CancellationToken cancellationToken = default);
}