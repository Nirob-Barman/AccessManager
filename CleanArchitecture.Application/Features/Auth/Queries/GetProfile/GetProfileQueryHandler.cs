using MediatR;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto>
{
    private readonly IIdentityService _identityService;

    public GetProfileQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _identityService.GetProfileAsync(request.UserId)
            ?? throw new NotFoundException("User", request.UserId);

        return new ProfileDto(profile.Email, profile.FullName, profile.PhoneNumber, profile.Address, profile.ProfileImageUrl, profile.Role);
    }
}
