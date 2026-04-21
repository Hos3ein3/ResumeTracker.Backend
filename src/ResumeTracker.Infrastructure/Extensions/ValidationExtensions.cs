using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ResumeTracker.API.Extensions;

public static class ValidationExtensions
{
    /// <summary>
    /// Adds FluentValidation as an endpoint filter on a minimal API route.
    /// Usage: .WithValidator<MyCommand>()
    /// /// </summary>
    public static RouteHandlerBuilder WithValidator<TRequest>(
        this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var validator = context.HttpContext.RequestServices
                .GetService<IValidator<TRequest>>();

            if (validator is null)
                return await next(context);

            var argument = context.Arguments.OfType<TRequest>().FirstOrDefault();
            if (argument is null)
                return await next(context);

            var result = await validator.ValidateAsync(argument);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(
                    result.ToDictionary(),
                    statusCode: StatusCodes.Status422UnprocessableEntity);
            }

            return await next(context);
        });
    }
}