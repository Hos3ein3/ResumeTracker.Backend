

using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Events.Users;

namespace ResumeTracker.Domain.Entities.Users;

/// <summary>
/// Represents a user in the system. Contains properties for user information and methods for user-related operations.
/// the Real User entity is in Persistence layer (ApplicationUser) which extends IdentityUser, this User entity is just for Domain logic and events, it won't be used for authentication/authorization directly.
/// </summary> <summary>
/// 
/// </summary>
public class User : AuditableAggregateRoot<Guid>
{

    public User(Guid id)
    {
        Id = id;
    }
    public User()
    {

    }

    public string? PhoneNumber { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }

    public string? SecurityStamp { get; set; }

    public static OperationResult<User> Create(string email, string userName,string phoneNumber, string firstName, string lastName,
    string? preferredLanguage,
    string? timeZone)
    {
        var user = new User
        {
            Id = NewId.Next(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = userName,
            PhoneNumber = phoneNumber,

        };
        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, email, firstName, lastName, preferredLanguage, timeZone));
        return OperationResult<User>.Success(user);
    }
}