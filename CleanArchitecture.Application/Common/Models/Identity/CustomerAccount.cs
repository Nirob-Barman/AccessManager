namespace CleanArchitecture.Application.Common.Models.Identity;

public record CustomerAccount(string Id, string Email, string FullName, string? PhoneNumber, string? Address, bool IsLockedOut);
