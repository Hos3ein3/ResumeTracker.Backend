

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ResumeTracker.Domain.Entities;

namespace ResumeTracker.Persistence.Configurations;

public sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes", DatabaseSchemas.App);

        // ─── Key ──────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        // ─── Identity ─────────────────────────────────────────────────
        builder.Property(x => x.UserId)
            .IsRequired();

        // ─── Job Info ─────────────────────────────────────────────────
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Position)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.Location)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.LinkUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.JobDescription)
            .HasMaxLength(1000)
            .IsRequired(false);

        // ─── Application Info ─────────────────────────────────────────
        builder.Property(x => x.ApplyDate)
            .IsRequired();

        builder.Property(x => x.ResumeSource)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ResumeStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Note)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.CoverLetterText)
        .HasMaxLength(1000)
        .IsRequired(false);

        // ─── Ignore ───────────────────────────────────────────────────
        builder.Ignore(x => x.DomainEvents);
    }
}
