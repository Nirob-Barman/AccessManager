namespace CleanArchitecture.Application.Features.RoleManagement.DTOs;

public record UserRoleDto(
    string UserId,
    string UserName,
    string Email,
    bool IsSelected
);