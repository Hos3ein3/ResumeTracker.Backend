// ResumeTracker.Application/Common/Models/PagedQuery.cs
namespace ResumeTracker.Application.Common.Models;

public record PagedQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public bool SortDesc { get; init; } = false;
    public string? Search { get; init; }

    public int Skip => (Page - 1) * PageSize;
}