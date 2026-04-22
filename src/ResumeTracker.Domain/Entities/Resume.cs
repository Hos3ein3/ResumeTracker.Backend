

using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Enums;
using ResumeTracker.Domain.Events;
using ResumeTracker.Domain.Events.Resume;

namespace ResumeTracker.Domain.Entities;

public sealed class Resume : AuditableAggregateRoot<Guid>
{

    // ─── Identity ─────────────────────────────────────────────────────
    public Guid UserId { get; private set; }

    // ─── Job Info ─────────────────────────────────────────────────────
    public string Title { get; private set; } = default!;
    public string CompanyName { get; private set; } = default!;
    public string Position { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public string LinkUrl { get; private set; } = default!;
    public string JobDescription { get; private set; } = default!;

    // ─── Application Info ─────────────────────────────────────────────
    public DateTime ApplyDate { get; private set; }
    public ResumeSource ResumeSource { get; private set; } = ResumeSource.Other;
    public ResumeStatus ResumeStatus { get; private set; }
    public string Note { get; private set; } = default!;

    public string CoverLetterText { get; private set; } = default!;

    // ─── EF Core ──────────────────────────────────────────────────────
    private Resume() { }

    // ─── Constructor ──────────────────────────────────────────────────
    private Resume(
        Guid userId,
        string title,
        string companyName,
        string position,
        string location,
        string linkUrl,
        string jobDescription,
        DateTime applyDate,
        ResumeSource resumeSource,
        ResumeStatus resumeStatus,
        string note,
        string coverLetterText)
    {
        Id = NewId.Next();
        UserId = userId;
        Title = title;
        CompanyName = companyName;
        Position = position;
        Location = location;
        LinkUrl = linkUrl;
        JobDescription = jobDescription;
        ApplyDate = applyDate;
        ResumeSource = resumeSource;
        ResumeStatus = resumeStatus;
        Note = note;
        CoverLetterText = coverLetterText;

        RaiseDomainEvent(new ResumeCreatedDomainEvent(Id, UserId));
    }

    // ─── Factory ──────────────────────────────────────────────────────
    public static Resume Create(
        Guid userId,
        string title,
        string companyName,
        string position,
        string location,
        string linkUrl,
        string jobDescription,
        DateTime applyDate,
        ResumeSource resumeSource,
        ResumeStatus resumeStatus,
        string note,
        string coverLetterText)
        => new(
            userId,
            title,
            companyName,
            position,
            location,
            linkUrl,
            jobDescription,
            applyDate,
            resumeSource,
            resumeStatus,
            note, coverLetterText);

    // ─── Behaviour ────────────────────────────────────────────────────
    public void UpdateStatus(ResumeStatus newStatus)
    {
        if (ResumeStatus == newStatus) return;

        var previous = ResumeStatus;
        ResumeStatus = newStatus;

        RaiseDomainEvent(new ResumeStatusChangedEvent(
            Id, previous, newStatus, DateTime.UtcNow));
    }

    public void Update(
        string title,
        string companyName,
        string position,
        string location,
        string linkUrl,
        string jobDescription,
        DateTime applyDate,
        ResumeSource resumeSource,
        string note, string coverLetterText)
    {
        Title = title;
        CompanyName = companyName;
        Position = position;
        Location = location;
        LinkUrl = linkUrl;
        JobDescription = jobDescription;
        ApplyDate = applyDate;
        ResumeSource = resumeSource;
        Note = note;
        CoverLetterText = coverLetterText;

        //RaiseDomainEvent(new ResumeUpdatedDomainEvent(Id, UserId));
    }

}
