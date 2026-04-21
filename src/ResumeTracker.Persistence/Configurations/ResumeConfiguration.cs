

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ResumeTracker.Domain.Entities;

namespace ResumeTracker.Persistence.Configurations;

public sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Position)
            .HasMaxLength(100);

        builder.Property(x => x.Note).HasMaxLength(500);

        builder.Property(x => x.JobDescription).HasMaxLength(1000);
        


        builder.Ignore(x => x.DomainEvents);
    }
}
