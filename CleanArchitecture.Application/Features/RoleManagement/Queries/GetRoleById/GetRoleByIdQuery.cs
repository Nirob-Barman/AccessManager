using MediatR;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Features.RoleManagement.Queries.GetRoleById;

public record GetRoleByIdQuery(string Id)
    : IRequest<RoleDetailsDto?>;