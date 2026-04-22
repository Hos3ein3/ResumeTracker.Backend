using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ResumeTracker.API.Extensions;
using ResumeTracker.API.OpenApi;
using ResumeTracker.Infrastructure;
using ResumeTracker.Infrastructure.Configurations;
using ResumeTracker.Infrastructure.Middlewares;
using ResumeTracker.Infrastructure.Middlewares.HttpLogging;
using ResumeTracker.Infrastructure.Settings;
using ResumeTracker.Persistence;
using ResumeTracker.Persistence.Identity;
using ResumeTracker.Persistence.Seeds;

using Scalar.AspNetCore;

using Serilog;


SerilogConfiguration.CreateBootstrapLogger();
try
{
    Log.Information("Starting ResumeTracker API...");
    var builder = WebApplication.CreateBuilder(args);



    // ── Stage 2: Full Serilog (reads appsettings.json) ────
    builder.Services.AddSerilog((services, loggerConfig) =>
        SerilogConfiguration.ConfigureLogger(
            builder.Host.GetType()
                .GetProperty("Context")?.GetValue(builder.Host) as HostBuilderContext
                ?? new HostBuilderContext(new Dictionary<object, object>())
                {
                    HostingEnvironment = builder.Environment,
                    Configuration = builder.Configuration
                },
            services,
            loggerConfig));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

    builder.Services.AddApiVersioningSetup();
    builder.Services.AddVersionedRouters(typeof(Program).Assembly);



    // ──────────────────────────────────────────────
    // Controllers
    // ──────────────────────────────────────────────
    builder.Services.AddControllers();

    // ──────────────────────────────────────────────
    // Persistence (PostgreSQL + Identity + Repos)
    // ──────────────────────────────────────────────
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    // ──────────────────────────────────────────────
    // JWT Authentication
    // ──────────────────────────────────────────────
    var jwtSettings = builder.Configuration
     .GetSection(JwtSettings.SectionName)
     .Get<JwtSettings>()!;

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero   // ← no grace period; exact expiry
            };

            // Return proper 401 JSON instead of redirect
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async ctx =>
                {
                    ctx.HandleResponse();
                    ctx.Response.StatusCode = 401;
                    ctx.Response.ContentType = "application/problem+json";
                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        status = 401,
                        title = "Unauthorized",
                        detail = "Access token is missing or invalid.",
                        instance = ctx.Request.Path.ToString()
                    });
                },
                OnForbidden = async ctx =>
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.ContentType = "application/problem+json";
                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        status = 403,
                        title = "Forbidden",
                        detail = "You do not have permission to access this resource.",
                        instance = ctx.Request.Path.ToString()
                    });
                }
            };
        });



    builder.Services.AddAuthorization();

    builder.Services.AddRouters();
    // ──────────────────────────────────────────────
    // Build
    // ──────────────────────────────────────────────
    var app = builder.Build();
    app.UseExceptionHandler();
    app.UseStructuredRequestLogging();
    app.UseCorrelationContext();
    app.UseLocalizationSetup();
    app.UseHttpRequestResponseLogging();


    app.MapOpenApi();  // exposes /openapi/v1.json

    app.MapScalarApiReference(options =>
 {
     options.Title = "ResumeTracker API";
     options.Theme = ScalarTheme.Purple;
     options.Authentication = new ScalarAuthenticationOptions
     {
         PreferredSecuritySchemes = ["Bearer"]
     };
 });

    app.MapVersionedRouters();
    app.UseHealthCheckEndpoints();
    // Redirect root to Health Check UI
    // app.MapGet("/", () => Results.Redirect("/health-ui"))
    //    .ExcludeFromDescription(); // hides it from Scalar/OpenAPI docs

    app.MapGet("/", () => Results.Redirect("/scalar"))
      .ExcludeFromDescription();


    app.UseHttpsRedirection();

    app.UseCorsPipeline();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Seed database on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ResumeTrackerDbContext>();

        await db.Database.MigrateAsync();   // apply pending migrations

        // UseAsyncSeeding is triggered automatically by EnsureCreatedAsync,
        // but with Migrate() you call it manually:
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        await IdentityDataSeeder.SeedAsync(userManager, roleManager);
    }


    app.Run();

}
catch (System.Exception ex)
{
    Log.Fatal(ex, "ResumeTracker API terminated unexpectedly.");
}
finally
{
    Log.Information("Shutting down ResumeTracker API.");
    Log.CloseAndFlush();
}


