// ResumeTracker.Domain/Common/PagedList.cs
namespace ResumeTracker.Domain.Common;

public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public PagedList(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));

        Items = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// Creates an empty paged list.
    /// </summary>
    public static PagedList<T> Empty(int page = 1, int pageSize = 10)
        => new([], 0, page, pageSize);

    /// <summary>
    /// Projects each item into a new form — useful for mapping domain → DTO.
    /// </summary>
    public PagedList<TResult> Map<TResult>(Func<T, TResult> selector)
        => new(Items.Select(selector), TotalCount, Page, PageSize);
}