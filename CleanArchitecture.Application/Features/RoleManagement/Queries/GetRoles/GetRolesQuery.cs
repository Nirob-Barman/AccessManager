using MediatR;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Features.RoleManagement.Queries.GetRoles;

public record GetRolesQuery
    : IRequest<List<RoleManagementDto>>;