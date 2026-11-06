namespace CleanArchitecture.Application.Common.Models.Identity;

public record RefreshTokenResult(bool Succeeded, string? UserId, string? NewRawToken, DateTime? NewExpiresAt);
