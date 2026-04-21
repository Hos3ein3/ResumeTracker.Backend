FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and all source
COPY *.slnx .
COPY src/ ./src/

# Restore
RUN dotnet restore src/ResumeTracker.API/ResumeTracker.API.csproj

# Publish
RUN dotnet publish src/ResumeTracker.API/ResumeTracker.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime ──────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8090

EXPOSE 8090

ENTRYPOINT ["dotnet", "ResumeTracker.API.dll"]