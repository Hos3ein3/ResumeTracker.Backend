namespace ResumeTracker.Domain.Enums;

public enum ResumeStatus : uint
{
    Applied = 1,
    PhoneScreen = 2,
    Interview = 3,
    Offer = 4,
    Rejected = 5,
    Ghosted = 6,
    Withdrawn = 7,
    OnHold = 8
}