namespace CleanArchitecture.Application.Common.Models.GoogleLogin;

public record GoogleUserInfo(
    string GoogleId,
    string Email,
    string? FullName,
    string? Picture);
