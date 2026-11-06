using MediatR;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Features.RoleManagement.Queries.GetUsersForRole;

public record GetUsersForRoleQuery(string RoleId)
    : IRequest<AssignUsersDto?>;