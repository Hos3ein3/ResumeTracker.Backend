using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ResumeTracker.API.Extensions;
using ResumeTracker.Infrastructure;
using ResumeTracker.Infrastructure.Configurations;
using ResumeTracker.Infrastructure.Middlewares;
using ResumeTracker.Infrastructure.Middlewares.HttpLogging;
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
    builder.Services.AddOpenApi();
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
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secretKey = builder.Configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey is missing.");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
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

    // ──────────────────────────────────────────────
    // Scalar UI — only in Development
    // ──────────────────────────────────────────────
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();  // exposes /openapi/v1.json

        app.MapScalarApiReference(options =>
     {
         options.Title = "ResumeTracker API";
         options.Theme = ScalarTheme.Purple;
         options.WithPreferredScheme("Bearer");
     });

        // ── Routes ─────────────────────────────────────────────

        // Scalar UI available at: /scalar/v1
    }
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


