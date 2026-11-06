using MediatR;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string Credential) : IRequest<AuthResponseDto>;