using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResumeTracker.Infrastructure.Extensions;


public static class EnumExtensions
{
    // ─── Display Name ────────────────────────────────────────────────

    /// <summary>
    /// Gets the [Display(Name = "...")] value, or falls back to the enum name.
    /// Supports localization via [Display(ResourceType = typeof(Resource), Name = "Key")].
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        if (member is null) return value.ToString();

        var display = member.GetCustomAttribute<DisplayAttribute>();
        if (display is null) return value.ToString();

        // Supports ResourceType for multi-language
        return display.GetName() ?? value.ToString();
    }

    /// <summary>
    /// Gets the [Display(Description = "...")] value.
    /// </summary>
    public static string? GetDisplayDescription(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<DisplayAttribute>()?.GetDescription();
    }

    /// <summary>
    /// Gets the [Display(ShortName = "...")] value.
    /// </summary>
    public static string? GetDisplayShortName(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<DisplayAttribute>()?.GetShortName();
    }

    /// <summary>
    /// Gets the [Display(Order = ...)] value.
    /// </summary>
    public static int GetDisplayOrder(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? int.MaxValue;
    }

    // ─── Conversion ───────────────────────────────────────────────────

    /// <summary>
    /// Converts an enum type to a list of its values.
    /// </summary>
    public static List<T> ToList<T>() where T : struct, Enum
        => Enum.GetValues<T>().ToList();

    /// <summary>
    /// Converts an enum type to a dictionary of (value → display name).
    /// Sorted by [Display(Order)] if present.
    /// </summary>
    public static Dictionary<T, string> ToDictionary<T>() where T : struct, Enum
        => Enum.GetValues<T>()
            .OrderBy(e => e.GetDisplayOrder())
            .ToDictionary(e => e, e => e.GetDisplayName());

    // ─── SelectListItem ───────────────────────────────────────────────

    /// <summary>
    /// Converts an enum type to a list of SelectListItem.
    /// /// </summary>
    public static List<SelectListItem> ToSelectList<T>() where T : struct, Enum
        => Enum.GetValues<T>()
            .OrderBy(e => e.GetDisplayOrder())
            .Select(e => new SelectListItem
            {
                Value = Convert.ToInt32(e).ToString(),
                Text = e.GetDisplayName()
            })
            .ToList();

    /// <summary>
    /// Converts an enum to SelectListItem list with an optional single selected value.
    /// Passing null means no item is pre-selected.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T>(T? selectedValue)
        where T : struct, Enum
        => Enum.GetValues<T>()
            .OrderBy(e => e.GetDisplayOrder())
            .Select(e => new SelectListItem
            {
                Value = Convert.ToInt32(e).ToString(),
                Text = e.GetDisplayName(),
                Selected = selectedValue.HasValue
                           && EqualityComparer<T>.Default.Equals(e, selectedValue.Value)
            })
            .ToList();

    /// <summary>
    /// Converts an enum to SelectListItem list with optional multiple selected values.
    /// Passing null or empty means no items are pre-selected.
    /// </summary>
    public static List<SelectListItem> ToSelectList<T>(IEnumerable<T>? selectedValues)
        where T : struct, Enum
    {
        var selected = selectedValues?.ToHashSet() ?? [];

        return Enum.GetValues<T>()
            .OrderBy(e => e.GetDisplayOrder())
            .Select(e => new SelectListItem
            {
                Value = Convert.ToInt32(e).ToString(),
                Text = e.GetDisplayName(),
                Selected = selected.Count > 0 && selected.Contains(e)
            })
            .ToList();
    }

    /// <summary>
    /// Adds a placeholder item at the top.
    /// If selectedValue is null, the placeholder is shown as selected.
    /// </summary>
    public static List<SelectListItem> ToSelectListWithPlaceholder<T>(
        string placeholderText = "-- Select --",
        T? selectedValue = null,
        bool disablePlaceholder = true)
        where T : struct, Enum
    {
        var items = ToSelectList(selectedValue);

        items.Insert(0, new SelectListItem
        {
            Value = "",
            Text = placeholderText,
            Disabled = disablePlaceholder,
            Selected = !selectedValue.HasValue   // ← pre-selects placeholder when null
        });

        return items;
    }

    // ─── Flags / Bitwise ─────────────────────────────────────────────

    /// <summary>
    /// Returns all individual flags set on a [Flags] enum value.
    /// </summary>
    public static IEnumerable<T> GetFlags<T>(this T value) where T : struct, Enum
        => Enum.GetValues<T>().Where(flag =>
        {
            var flagInt = Convert.ToInt64(flag);
            return flagInt != 0 && (Convert.ToInt64(value) & flagInt) == flagInt;
        });

    /// <summary>
    /// Checks if a [Flags] enum value contains a specific flag.
    /// </summary>
    public static bool HasFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var v = Convert.ToInt64(value);
        var f = Convert.ToInt64(flag);
        return f != 0 && (v & f) == f;
    }

    // ─── Parsing ──────────────────────────────────────────────────────

    /// <summary>
    /// Safely parses a string to an enum value. Returns null if invalid.
    /// </summary>
    public static T? ParseOrNull<T>(string? value) where T : struct, Enum
        => Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : null;

    /// <summary>
    /// Safely parses an int to an enum value. Returns null if not defined.
    /// </summary>
    public static T? FromIntOrNull<T>(int? value) where T : struct, Enum
        => value.HasValue && Enum.IsDefined(typeof(T), value.Value)
            ? (T)Enum.ToObject(typeof(T), value.Value)
            : null;
}
