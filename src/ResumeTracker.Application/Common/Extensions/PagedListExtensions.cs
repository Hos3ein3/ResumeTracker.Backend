// ResumeTracker.Application/Common/Extensions/PagedListExtensions.cs
using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Common.Models;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Common.Extensions;

public static class PagedListExtensions
{
    // ─── IQueryable (EF Core) ─────────────────────────────────────────

    /// <summary>
    /// Async EF Core — executes COUNT + paged SELECT in two queries.
    /// </summary>
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<T>(items, totalCount, page, pageSize);
    }

    /// <summary>
    /// Async overload accepting a PagedQuery record directly.
    /// </summary>
    public static Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        PagedQuery pagedQuery,
        CancellationToken ct = default)
        => query.ToPagedListAsync(pagedQuery.Page, pagedQuery.PageSize, ct);

    // ─── IEnumerable (in-memory) ──────────────────────────────────────

    /// <summary>
    /// In-memory paging — use only on already-materialized collections.
    /// </summary>
    public static PagedList<T> ToPagedList<T>(
        this IEnumerable<T> source,
        int page,
        int pageSize)
    {
        var list = source.ToList();
        var totalCount = list.Count;
        var items = list
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return new PagedList<T>(items, totalCount, page, pageSize);
    }

    /// <summary>
    /// In-memory overload accepting a PagedQuery record.
    /// </summary>
    public static PagedList<T> ToPagedList<T>(
        this IEnumerable<T> source,
        PagedQuery pagedQuery)
        => source.ToPagedList(pagedQuery.Page, pagedQuery.PageSize);

    // ─── Mapping ──────────────────────────────────────────────────────

    /// <summary>
    /// Projects PagedList<TSource> → PagedList<TDest>.
    /// Use in Application layer to map domain entities → DTOs.
    /// </summary>
    public static PagedList<TDest> MapTo<TSource, TDest>(
        this PagedList<TSource> source,
        Func<TSource, TDest> selector)
        => source.Map(selector);

    /// <summary>
    /// Converts PagedList<T> → PagedResult<T> for API responses.
    /// </summary>
    public static PagedResult<T> ToPagedResult<T>(this PagedList<T> source)
        => new()
        {
            Items = source.Items,
            Page = source.Page,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount,
            TotalPages = source.TotalPages,
            HasPrevious = source.HasPrevious,
            HasNext = source.HasNext
        };

    /// <summary>
    /// Maps domain entities to DTOs and wraps as PagedResult in one call.
    /// </summary>
    public static PagedResult<TDest> ToPagedResult<TSource, TDest>(
        this PagedList<TSource> source,
        Func<TSource, TDest> selector)
        => source.Map(selector).ToPagedResult();
}