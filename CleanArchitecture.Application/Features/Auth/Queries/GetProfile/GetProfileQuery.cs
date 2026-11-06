using MediatR;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Queries.GetProfile;

public record GetProfileQuery(string UserId) : IRequest<ProfileDto>;
