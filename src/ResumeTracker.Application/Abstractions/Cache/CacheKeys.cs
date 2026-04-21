namespace ResumeTracker.Application.Abstractions.Cache;

public static class CacheKeys
{
    public static string Resume(Guid id) => $"resume:{id}";
    public static string UserResumes(Guid userId) => $"user:{userId}:resumes";
    public static string Permissions(Guid userId) => $"user:{userId}:permissions";
    public static string Role(Guid roleId) => $"role:{roleId}";
}