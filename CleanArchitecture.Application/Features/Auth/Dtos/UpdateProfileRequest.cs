namespace CleanArchitecture.Application.Features.Auth.Dtos;

public record UpdateProfileRequest(string FullName, string? PhoneNumber, string? Address, string? ProfileImageUrl);
