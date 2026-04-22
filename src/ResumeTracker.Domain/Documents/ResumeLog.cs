using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Domain.Documents;

public sealed class ResumeLog : MongoAggregateRoot
{
    public Guid ResumeId { get; set; }
    public DateTime ResumeStatusModifiedAt { get; set; } = DateTime.UtcNow;
    public ResumeStatus PreviousStatus { get; set; }
    public ResumeStatus CurrentStatus { get; set; }

    public static ResumeLog Create(Guid resumeId, ResumeStatus previousStatus, ResumeStatus currentStatus)
    => new()
    {
        ResumeId = resumeId,
        PreviousStatus = previousStatus,
        CurrentStatus = currentStatus
    };

}
