using MediatR;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.UpdateRole;

public class updateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, (bool Succeeded, string[] Errors)>
{
    private readonly IRoleManagementService _service;

    public updateRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<(bool Succeeded, string[] Errors)> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        return await _service.UpdateRoleAsync(request.Id, request.Name);
    }
}