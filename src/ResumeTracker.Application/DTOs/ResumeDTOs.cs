
using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Application.DTOs;


public record UserResumesResponse(
    Guid Id,
    string Title,
    string CompanyName,
    string Position,
    string Location,
    string LinkUrl,
    ResumeSource ResumeSource,
    ResumeStatus ResumeStatus,
    DateTime ApplyDate,
    string Note
);