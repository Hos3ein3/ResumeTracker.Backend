using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using ResumeTracker.Domain.Documents;
using ResumeTracker.Infrastructure.Settings;

namespace ResumeTracker.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    static MongoDbContext()
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("ResumeTrackerConventions", pack, _ => true);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));


    }


    public MongoDbContext(IMongoClient client, MongoDbSettings settings)
    {
        _database = client.GetDatabase(settings.DatabaseName);
        EnsureIndexes();
    }

    public IMongoCollection<ResumeFile> ResumeFiles =>
        _database.GetCollection<ResumeFile>("resume_files");

    public IMongoCollection<UserPreferences> UserPreferences =>
        _database.GetCollection<UserPreferences>("user_preferences");

    private IMongoCollection<T> _collection<T>(string name) =>
            _database.GetCollection<T>(name);
    private void EnsureIndexes()
    {
        ResumeFiles.Indexes.CreateOne(
            new CreateIndexModel<ResumeFile>(
                Builders<ResumeFile>.IndexKeys.Ascending(x => x.ResumeId)));

        ResumeFiles.Indexes.CreateOne(
            new CreateIndexModel<ResumeFile>(
                Builders<ResumeFile>.IndexKeys.Ascending(x => x.UserId)));

        UserPreferences.Indexes.CreateOne(new CreateIndexModel<UserPreferences>(
        Builders<UserPreferences>.IndexKeys.Ascending(x => x.UserId),
        new CreateIndexOptions { Unique = true }));
    }
}