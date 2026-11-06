namespace CleanArchitecture.Application.Common.Models.Identity;

public record AuthenticatedUser(string UserId, string Email, string FullName, string Role);
