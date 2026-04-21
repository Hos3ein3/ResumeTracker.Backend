

using ResumeTracker.Application.Features.Auth.Register;



namespace ResumeTracker.API.Features.Auth;

public static class Register
{
    public static async Task<IResult> Handle(
    RegisterRequest request,
    IAuthService authService,
    CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.ToHttpResult();
    }
}
