using CleanArchitecture.Application.Common.Models.Authorization;

namespace CleanArchitecture.Application.Features.RolePermissions.Dtos;

public record RolePermissionDto(
    string RoleId,
    List<RoleDto> Roles,
    List<MenuItem> Menus,
    List<string> AssignedPermissions
);