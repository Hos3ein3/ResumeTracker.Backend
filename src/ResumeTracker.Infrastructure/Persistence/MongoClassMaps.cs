using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

using ResumeTracker.Domain;
using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Infrastructure.Persistence;

public static class MongoClassMaps
{
    public static void Register()
    {
        // Base mapping — applies Id as _id for all MongoAggregateRoot types
        if (!BsonClassMap.IsClassMapRegistered(typeof(MongoAggregateRoot)))
        {
            BsonClassMap.RegisterClassMap<MongoAggregateRoot>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(x => x.Id)
                  .SetSerializer(new StringSerializer(BsonType.String));
                cm.SetIgnoreExtraElements(true);
            });
        }

        // Per-entity mappings
        if (!BsonClassMap.IsClassMapRegistered(typeof(ResumeLog)))
        {
            BsonClassMap.RegisterClassMap<ResumeLog>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}