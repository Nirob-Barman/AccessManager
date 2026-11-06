using MediatR;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var updated = await _identityService.UpdateProfileAsync(
            request.UserId,
            request.FullName,
            request.PhoneNumber,
            request.Address,
            request.ProfileImageUrl);

        if (!updated)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var profile = await _identityService.GetProfileAsync(request.UserId)
            ?? throw new NotFoundException("User", request.UserId);

        return new ProfileDto(profile.Email, profile.FullName, profile.PhoneNumber, profile.Address, profile.ProfileImageUrl, profile.Role);
    }
}
