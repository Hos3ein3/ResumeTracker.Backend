

namespace ResumeTracker.Domain.Exceptions;

public static class UserErrors
{
    public static readonly Error EmailAlreadyExists =
            new("User.EmailAlreadyExists", "A user with this email already exists.");

    public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Email or password is incorrect.");

    public static readonly Error NotFound =
        new("User.NotFound", "User was not found.");
}
