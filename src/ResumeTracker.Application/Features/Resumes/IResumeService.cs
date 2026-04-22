

using ResumeTracker.Application.Common.Models;
using ResumeTracker.Application.DTOs;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Application.Features.Resumes;

public interface IResumeService
{
    Task<OperationResult<PagedResult<UserResumesResponse>>> GetAllByUserAsync(
            Guid userId,
            PagedQuery query,
            CancellationToken ct = default);

    Task<OperationResult> UpdateResumeStatusAsync(Guid resumeId, ResumeStatus newStatus, CancellationToken ct = default);
}
