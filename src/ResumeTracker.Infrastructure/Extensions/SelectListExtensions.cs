
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResumeTracker.Infrastructure.Extensions;

public static class SelectListExtensions
{
    // ─── From List<T> with key/text selectors ─────────────────────────

    /// <summary>
    /// Converts a List<T> to SelectListItems with no pre-selection.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T>(
        this IEnumerable<T> source,
        Func<T, string> valueSelector,
        Func<T, string> textSelector)
        => source
            .Select(item => new SelectListItem
            {
                Value = valueSelector(item),
                Text = textSelector(item)
            })
            .ToList();

    /// <summary>
    /// Converts a List<T> to SelectListItems with a single pre-selected value.
    /// Passing null means no item is pre-selected.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T>(
        this IEnumerable<T> source,
        Func<T, string> valueSelector,
        Func<T, string> textSelector,
        string? selectedValue)
        => source
            .Select(item =>
            {
                var value = valueSelector(item);
                return new SelectListItem
                {
                    Value = value,
                    Text = textSelector(item),
                    Selected = selectedValue is not null && value == selectedValue
                };
            })
            .ToList();

    /// <summary>
    /// Converts a List<T> to SelectListItems with multiple pre-selected values.
    /// Passing null or empty means no items are pre-selected.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T>(
        this IEnumerable<T> source,
        Func<T, string> valueSelector,
        Func<T, string> textSelector,
        IEnumerable<string>? selectedValues)
    {
        var selected = selectedValues?.ToHashSet() ?? [];

        return source
            .Select(item =>
            {
                var value = valueSelector(item);
                return new SelectListItem
                {
                    Value = value,
                    Text = textSelector(item),
                    Selected = selected.Count > 0 && selected.Contains(value)
                };
            })
            .ToList();
    }

    // ─── Strongly-typed key overloads (int, Guid, etc.) ───────────────

    /// <summary>
    /// Overload for strongly-typed keys (e.g. int, Guid).
    /// Single pre-selected value. Null means no selection.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> valueSelector,
        Func<T, string> textSelector,
        TKey? selectedValue = default)
        where TKey : struct
        => source
            .Select(item =>
            {
                var value = valueSelector(item);
                return new SelectListItem
                {
                    Value = value.ToString()!,
                    Text = textSelector(item),
                    Selected = selectedValue.HasValue
                               && EqualityComparer<TKey>.Default.Equals(value, selectedValue.Value)
                };
            })
            .ToList();

    /// <summary>
    /// Overload for strongly-typed keys with multiple pre-selected values.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> valueSelector,
        Func<T, string> textSelector,
        IEnumerable<TKey>? selectedValues)
        where TKey : struct
    {
        var selected = selectedValues?.ToHashSet() ?? [];

        return source
            .Select(item =>
            {
                var value = valueSelector(item);
                return new SelectListItem
                {
                    Value = value.ToString()!,
                    Text = textSelector(item),
                    Selected = selected.Count > 0 && selected.Contains(value)
                };
            })
            .ToList();
    }

    // ─── Grouping ─────────────────────────────────────────────────────

    /// <summary>
    /// Converts a List<T> to grouped SelectListItems.
    /// </summary>
    public static List<SelectListItem> ToGroupedSelectList<T>(
        this IEnumerable<T> source,
        Func<T, string> valueSelector,
        Func<T, string> textSelector,
        Func<T, string> groupSelector,
        string? selectedValue = null)
    {
        var groups = new Dictionary<string, SelectListGroup>();

        return source
            .Select(item =>
            {
                var groupName = groupSelector(item);
                if (!groups.TryGetValue(groupName, out var group))
                {
                    group = new SelectListGroup { Name = groupName };
                    groups[groupName] = group;
                }

                var value = valueSelector(item);
                return new SelectListItem
                {
                    Value = value,
                    Text = textSelector(item),
                    Group = group,
                    Selected = selectedValue is not null && value == selectedValue
                };
            })
            .ToList();
    }

    // ─── Placeholder ─────────────────────────────────────────────────

    /// <summary>
    /// Prepends a placeholder item to an existing SelectListItem list.
    /// Placeholder is auto-selected when selectedValue is null.
    /// </summary>
    public static List<SelectListItem> WithPlaceholder(
        this List<SelectListItem> items,
        string placeholderText = "-- Select --",
        string placeholderValue = "",
        bool disablePlaceholder = true)
    {
        var hasSelection = items.Any(i => i.Selected);

        items.Insert(0, new SelectListItem
        {
            Value = placeholderValue,
            Text = placeholderText,
            Disabled = disablePlaceholder,
            Selected = !hasSelection
        });

        return items;
    }

    // ─── Disable items ────────────────────────────────────────────────

    /// <summary>
    /// Disables specific items in a SelectListItem list by value.
    /// </summary>
    public static List<SelectListItem> WithDisabled(
        this List<SelectListItem> items,
        IEnumerable<string> disabledValues)
    {
        var disabled = disabledValues.ToHashSet();
        foreach (var item in items.Where(i => disabled.Contains(i.Value)))
            item.Disabled = true;

        return items;
    }
}
