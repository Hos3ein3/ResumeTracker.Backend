

using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Identity;

using ResumeTracker.Domain;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Events.Users;
using ResumeTracker.Persistence.Entities;

namespace ResumeTracker.Persistence.Identity;

public class ApplicationUser : IdentityUser<Guid>, IHasDomainEvents
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;


    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    // ─── Domain Events (manually since we can't extend AggregateRoot) ─
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    private void RaiseDomainEvent(IDomainEvent e) => _domainEvents.Add(e);

    public static ApplicationUser Create(string email, string firstName, string lastName,
    string? preferredLanguage,
    string? timeZone)
    {
        var user = new ApplicationUser
        {
            Id = NewId.Next(),
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };
        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, email, firstName, lastName, preferredLanguage, timeZone));
        return user;
    }


}
