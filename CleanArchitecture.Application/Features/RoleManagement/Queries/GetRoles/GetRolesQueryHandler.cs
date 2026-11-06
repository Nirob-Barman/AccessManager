using MediatR;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Features.RoleManagement.Queries.GetRoles;

public class GetRolesQueryHandler
    : IRequestHandler<GetRolesQuery, List<RoleManagementDto>>
{
    private readonly IRoleManagementService _service;

    public GetRolesQueryHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<List<RoleManagementDto>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetRolesAsync();
    }
}