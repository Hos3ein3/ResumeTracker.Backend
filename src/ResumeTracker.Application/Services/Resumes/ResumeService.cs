

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Common.Extensions;
using ResumeTracker.Application.Common.Models;
using ResumeTracker.Application.DTOs;
using ResumeTracker.Application.Features.Resumes;
using ResumeTracker.Application.Repositories;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Application.Services.Resumes;

public class ResumeService(IResumeRepository repository, IUnitOfWork unitOfWork) : IResumeService
{
    public async Task<OperationResult<PagedResult<UserResumesResponse>>> GetAllByUserAsync(Guid userId, PagedQuery query, CancellationToken ct = default)
    {
        var pagedList = await repository.GetAllByUserAsync(userId, query, ct);

        var result = pagedList.ToPagedResult(resume => new UserResumesResponse(
            Id: resume.Id,
            Title: resume.Title,
            CompanyName: resume.CompanyName,
            Position: resume.Position,
            Location: resume.Location,
            LinkUrl: resume.LinkUrl,
            ResumeSource: resume.ResumeSource,
            ResumeStatus: resume.ResumeStatus,
            ApplyDate: resume.ApplyDate,
            Note: resume.Note
        ));

        return OperationResult<PagedResult<UserResumesResponse>>.Success(result);
    }

    public async Task<OperationResult> UpdateResumeStatusAsync(Guid resumeId, ResumeStatus newStatus, CancellationToken ct = default)
    {
        var resume = await repository.GetByIdAsync(resumeId);
        if (resume is null) return OperationResult.NotFound($"Resume '{resumeId}' not found.");

        if (resume.ResumeStatus == newStatus) return OperationResult.Info("Statuses are same");

        resume.UpdateStatus(newStatus);

        repository.Update(resume);
        await unitOfWork.SaveChangesAsync(ct);
        return OperationResult.Success();
    }
}
