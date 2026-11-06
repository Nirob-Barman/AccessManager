namespace CleanArchitecture.Application.Features.Auth.Dtos;

public record AuthResponseDto(
    string UserId,
    string Email,
    string FullName,
    string Role
);