using MediatR;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(string UserId, string FullName, string? PhoneNumber, string? Address, string? ProfileImageUrl) : IRequest<ProfileDto>;
