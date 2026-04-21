using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using ResumeTracker.Persistence.ValueGenerators;

namespace ResumeTracker.Persistence.Conventions;

/// <summary>
/// Applies GuidV7ValueGenerator to every Guid primary key property automatically.
/// No need to configure per-entity — it covers all current and future entities.
/// </summary>
public sealed class GuidV7Convention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (var key in entityType.GetKeys().Where(k => k.IsPrimaryKey()))
            {
                foreach (var property in key.Properties)
                {
                    if (property.ClrType == typeof(Guid))
                    {
                        property.Builder.HasValueGenerator(
                            (_, _) => new GuidV7ValueGenerator());
                    }
                }
            }
        }
    }
}