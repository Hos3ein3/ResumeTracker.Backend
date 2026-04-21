namespace ResumeTracker.Application.Abstractions.Security;

public interface ICurrentUserService
{
    Guid UserId { get; }
    uint Priority { get; }     // lowest number = highest authority
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Permissions { get; }
}