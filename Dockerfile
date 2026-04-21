FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy each .csproj individually (skip .slnx)
COPY src/ResumeTracker.API/ResumeTracker.API.csproj                         src/ResumeTracker.API/
COPY src/ResumeTracker.Application/ResumeTracker.Application.csproj         src/ResumeTracker.Application/
COPY src/ResumeTracker.Domain/ResumeTracker.Domain.csproj                   src/ResumeTracker.Domain/
COPY src/ResumeTracker.Infrastructure/ResumeTracker.Infrastructure.csproj   src/ResumeTracker.Infrastructure/
COPY src/ResumeTracker.Persistence/ResumeTracker.Persistence.csproj         src/ResumeTracker.Persistence/

# Restore
RUN dotnet restore src/ResumeTracker.API/ResumeTracker.API.csproj

# Copy full source and publish
COPY src/ ./src/
RUN dotnet publish src/ResumeTracker.API/ResumeTracker.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime ──────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8090

EXPOSE 8090

ENTRYPOINT ["dotnet", "ResumeTracker.API.dll"]