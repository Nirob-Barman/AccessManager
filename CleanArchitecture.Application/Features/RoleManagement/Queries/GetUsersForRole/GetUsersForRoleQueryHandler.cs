using MediatR;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Features.RoleManagement.Queries.GetUsersForRole;

public class GetUsersForRoleQueryHandler
    : IRequestHandler<GetUsersForRoleQuery, AssignUsersDto?>
{
    private readonly IRoleManagementService _service;

    public GetUsersForRoleQueryHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<AssignUsersDto?> Handle(
        GetUsersForRoleQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetUsersForRoleAsync(request.RoleId);
    }
}