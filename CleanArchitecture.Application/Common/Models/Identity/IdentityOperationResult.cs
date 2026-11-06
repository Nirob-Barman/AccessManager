namespace CleanArchitecture.Application.Common.Models.Identity;

public record IdentityOperationResult(bool Succeeded, string? UserId, IReadOnlyCollection<string> Errors);
