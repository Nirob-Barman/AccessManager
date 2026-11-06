using MediatR;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password, bool RememberMe = false) : IRequest<AuthResponseDto>;
