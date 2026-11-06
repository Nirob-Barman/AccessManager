namespace CleanArchitecture.Application.Features.RoleManagement.DTOs;

public record RoleManagementDto(
    string Id,
    string Name,
    int UserCount
);