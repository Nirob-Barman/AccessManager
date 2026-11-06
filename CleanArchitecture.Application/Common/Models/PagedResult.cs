namespace CleanArchitecture.Application.Common.Models;

public record PagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount, int PageNumber, int PageSize);
