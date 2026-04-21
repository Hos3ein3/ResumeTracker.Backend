namespace ResumeTracker.Domain.Enums;

public enum ResumeSource : uint
{
    LinkedIn = 1,
    Indeed = 2,
    Glassdoor = 3,
    Referral = 4,
    CompanyWebsite = 5,
    CareerService = 6,
    Other = uint.MaxValue
}