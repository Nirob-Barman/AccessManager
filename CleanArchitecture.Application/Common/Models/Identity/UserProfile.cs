namespace CleanArchitecture.Application.Common.Models.Identity;

public record UserProfile(string UserId, string Email, string FullName, string? PhoneNumber, string? Address, string? ProfileImageUrl, string Role);
