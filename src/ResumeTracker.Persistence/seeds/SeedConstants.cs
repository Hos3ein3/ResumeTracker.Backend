namespace ResumeTracker.Persistence.Seeds;

public static class SeedConstants
{
    public static class Roles
    {
        public static readonly Guid AdminId = Guid.Parse("11111111-0000-0000-0000-000000000001");
        public static readonly Guid DeveloperId = Guid.Parse("11111111-0000-0000-0000-000000000002");
        public static readonly Guid SystemId = Guid.Parse("11111111-0000-0000-0000-000000000003");
    }

    public static class Users
    {
        public static readonly Guid AdminId = Guid.Parse("22222222-0000-0000-0000-000000000001");
        public static readonly Guid DeveloperId = Guid.Parse("22222222-0000-0000-0000-000000000002");
        public static readonly Guid SystemId = Guid.Parse("22222222-0000-0000-0000-000000000003");
    }
}