

using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Enums;
using ResumeTracker.Domain.Events;

namespace ResumeTracker.Domain.Entities;

public sealed class Resume : AuditableAggregateRoot<Guid>
{
    public string Title { get; private set; } = default!;
    public string CompanyName { get; private set; }
    public string Position { get; private set; }
    public string Location { get; private set; }

    public ResumeSource ResumeSource { get; private set; } = ResumeSource.Other;
    public ResumeStatus ResumeStatus { get; private set; }
    public string LinkUrl { get; private set; }

    public DateTime ApplyDate { get; private set; }
    public string Note { get; private set; }

    public string JobDescription { get; private set; }
    public Guid UserId { get; private set; }

    private Resume() { }

    public Resume(Guid id, string title, Guid userId, string companyName, string position, string location, ResumeSource resumeSource, ResumeStatus resumeStatus, string linkUrl, string note,
        DateTime applyDate, string jobDescription)
    {
        Id = NewId.Next();
        Title = title;
        UserId = userId;
        CompanyName = companyName;
        Position = position;
        Location = location;
        LinkUrl = linkUrl;
        Note = note;
        ApplyDate = applyDate;
        ResumeStatus = resumeStatus;
        JobDescription = jobDescription;
        


        RaiseDomainEvent(new ResumeCreatedDomainEvent(Id, UserId));
    }

    public void Rename(string title) => Title = title;

}