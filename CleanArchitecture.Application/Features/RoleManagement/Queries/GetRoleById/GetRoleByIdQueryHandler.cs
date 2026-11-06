using MediatR;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.RoleManagement.DTOs;

namespace CleanArchitecture.Application.Features.RoleManagement.Queries.GetRoleById;

public class GetRoleByIdQueryHandler
    : IRequestHandler<GetRoleByIdQuery, RoleDetailsDto?>
{
    private readonly IRoleManagementService _service;

    public GetRoleByIdQueryHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<RoleDetailsDto?> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetRoleByIdAsync(request.Id);
    }
}