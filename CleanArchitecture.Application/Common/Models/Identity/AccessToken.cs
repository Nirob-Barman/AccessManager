namespace CleanArchitecture.Application.Common.Models.Identity;

public record AccessToken(string Token, DateTime ExpiresAt);
