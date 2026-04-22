// ResumeTracker.Application/Common/Models/PagedResult.cs
namespace ResumeTracker.Application.Common.Models;

/// <summary>
/// Serialization-friendly DTO — safe to return from API controllers.
/// Created from a PagedList<T> via .ToPagedResult().
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }

    public static PagedResult<T> Empty(int page = 1, int pageSize = 10) => new()
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = 0,
        TotalPages = 0
    };
}