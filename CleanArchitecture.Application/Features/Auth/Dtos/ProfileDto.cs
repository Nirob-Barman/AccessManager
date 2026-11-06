namespace CleanArchitecture.Application.Features.Auth.Dtos;

public record ProfileDto(string Email, string FullName, string? PhoneNumber, string? Address, string? ProfileImageUrl, string Role);
