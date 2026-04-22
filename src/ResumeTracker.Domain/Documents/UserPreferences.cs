using ResumeTracker.Domain.Common;

namespace ResumeTracker.Domain.Documents;

public sealed class UserPreferences : MongoAggregateRoot
{
    public Guid UserId { get; set; }               // FK to Identity user

    // ── UI / Localization ──────────────────────────────────
    public string Language { get; set; } = "en";    // "en", "fa", "it"
    public string Theme { get; set; } = "light"; // "light", "dark", "system"
    public string TimeZone { get; set; } = "UTC";   // IANA tz: "Asia/Tehran"
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public bool UseRtlLayout { get; set; } = false;

    // ── Notification preferences ───────────────────────────
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = false;
    public bool ResumeViewAlerts { get; set; } = true;

    // ── Resume display preferences ─────────────────────────
    public int DefaultPageSize { get; set; } = 20;
    public string DefaultSortBy { get; set; } = "CreatedAtUtc";
    public string DefaultSortOrder { get; set; } = "desc";

}