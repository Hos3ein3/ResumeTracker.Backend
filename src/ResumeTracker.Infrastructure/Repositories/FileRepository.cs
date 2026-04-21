using MongoDB.Bson;
using MongoDB.Driver;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain.Documents;
using ResumeTracker.Infrastructure.Persistence;

namespace ResumeTracker.Infrastructure.Repositories;

public sealed class FileRepository : IFileRepository
{
    private readonly MongoDbContext _context;

    public FileRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeFile?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ResumeFiles
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ResumeFile>> GetByResumeIdAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ResumeFiles
            .Find(x => x.ResumeId == resumeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<string> AddAsync(
        ResumeFile file,
        CancellationToken cancellationToken = default)
    {
        file.Id = ObjectId.GenerateNewId().ToString();
        await _context.ResumeFiles.InsertOneAsync(file, cancellationToken: cancellationToken);
        return file.Id;
    }

    public async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        await _context.ResumeFiles.DeleteOneAsync(
            x => x.Id == id, cancellationToken);
    }

    public async Task DeleteByResumeIdAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        await _context.ResumeFiles.DeleteManyAsync(
            x => x.ResumeId == resumeId, cancellationToken);
    }
}