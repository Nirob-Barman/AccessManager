using MediatR;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.CreateRole;

public class CreateRoleCommandHandler
    : IRequestHandler<CreateRoleCommand, (bool Succeeded, string[] Errors)>
{
    private readonly IRoleManagementService _service;

    public CreateRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<(bool Succeeded, string[] Errors)> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        return await _service.CreateRoleAsync(request.Name);
    }
}