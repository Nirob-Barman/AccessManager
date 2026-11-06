using MediatR;
using CleanArchitecture.Application.Features.Auth.Dtos;

namespace CleanArchitecture.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FullName, string? PhoneNumber) : IRequest<AuthResponseDto>;
