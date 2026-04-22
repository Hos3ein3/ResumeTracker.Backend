

using System.Runtime.CompilerServices;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using Moq;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Repositories;
using ResumeTracker.Application.Services.Resumes;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities;
using ResumeTracker.Domain.Enums;
using ResumeTracker.Domain.Events.Resume;

using Xunit;

namespace ResumeTracker.UnitTests.Application.Resumes;

public class ResumeServiceTests
{
    // AutoFixture creates objects automatically + AutoMoq creates mocks automatically
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly Mock<IResumeRepository> _resumeRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ResumeService _sut;           // sut = System Under Test
    public ResumeServiceTests()
    {
        _resumeRepoMock = _fixture.Freeze<Mock<IResumeRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();

        // ✅ AutoFixture creates a mock of IResumeService automatically
        _sut = new ResumeService(
             _resumeRepoMock.Object, _unitOfWorkMock.Object);
    }


    [Fact]
    public async Task UpdateResumeStatusAsync_WhenResumeNotFound_ShouldReturnNotFound()
    {
        // Arrange — mock returns null (resume doesn't exist)
        _resumeRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Resume?)null);

        // Act
        var result = await _sut.UpdateResumeStatusAsync(Guid.NewGuid(), ResumeStatus.Ghosted);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }



    [Fact]
    public async Task UpdateResumeStatusAsync_WhenResumeExist_ShouldUpdateAndSave()
    {
        //Arrange
        var resume = CreateResumeInstance();
        _resumeRepoMock.Setup(x => x.GetByIdAsync(resume.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resume);

        //Act

        var result = await _sut.UpdateResumeStatusAsync(resume.Id, ResumeStatus.Offer);


        //Assert
        result.IsSuccess.Should().BeTrue();
        resume.ResumeStatus.Should().Be(ResumeStatus.Offer);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateResumeStatusAsync_WhenStatusIsSame_ShouldNotSave()
    {
        //Arrange
        var resume = CreateResumeInstance();
        _resumeRepoMock.Setup(x => x.GetByIdAsync(resume.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resume);

        //Act
        var result = await _sut.UpdateResumeStatusAsync(resume.Id, ResumeStatus.Applied);

        //Assert
        result.Status.Should().Be(OperationResultStatus.Info);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact]
    public async Task UpdateResumeStatusAsync_WhenResumeExist_RaiseEvent()
    {
        //Arrange
        var resume = CreateResumeInstance();
        _resumeRepoMock.Setup(x => x.GetByIdAsync(resume.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resume);

        //Act
        var result = await _sut.UpdateResumeStatusAsync(resume.Id, ResumeStatus.Ghosted);

        //Assert
        resume.DomainEvents.Should().HaveCount(1);
        resume.DomainEvents.First().Should().BeOfType<ResumeStatusChangedEvent>();
    }

    private static Resume CreateResumeInstance()
    {
        var resume = Resume.Create(
            userId: Guid.NewGuid(),
            title: "Software Engineer at Google",
            companyName: "Google",
            position: "Backend Engineer",
            location: "Remote",
            linkUrl: "",
            jobDescription: "",
            applyDate: DateTime.UtcNow,
            resumeSource: ResumeSource.LinkedIn,
            resumeStatus: ResumeStatus.Applied,
            note: "",
            coverLetterText: "");
        resume.ClearDomainEvents();
        return resume;
    }
}
