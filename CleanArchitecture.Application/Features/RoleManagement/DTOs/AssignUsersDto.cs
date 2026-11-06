namespace CleanArchitecture.Application.Features.RoleManagement.DTOs;

public record AssignUsersDto(
    string RoleId,
    string RoleName,
    List<UserRoleDto> Users
);