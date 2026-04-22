
using FluentAssertions;

using ResumeTracker.Domain.Entities;
using ResumeTracker.Domain.Enums;
using ResumeTracker.Domain.Events.Resume;

using Xunit;

namespace ResumeTracker.UnitTests.Domain
{


    public class ResumeTests
    {




        // ─── Test 1: Status actually changes ──────────────────────────────
        [Fact]
        public void UpdateStatus_WhenNewStatusIsDifferent_ShouldUpdateStatus()
        {
            // Arrange
            var resume = CreateResumeInstance();

            // Act
            resume.UpdateStatus(ResumeStatus.Offer);

            // Assert
            resume.ResumeStatus.Should().Be(ResumeStatus.Offer);
        }

        // ─── Test 2: Event is raised ──────────────────────────────────────
        [Fact]
        public void UpdateStatus_WhenNewStatusIsDifferent_ShouldRaiseDomainEvent()
        {
            // Arrange
            var resume = CreateResumeInstance();

            // Act
            resume.UpdateStatus(ResumeStatus.Interview);

            // Assert — check an event was raised
            resume.DomainEvents.Should().HaveCount(1);
            resume.DomainEvents.First().Should().BeOfType<ResumeStatusChangedEvent>();
        }


        // ─── Test 3: Event contains correct data ─────────────────────────
        [Fact]
        public void UpdateStatus_WhenNewStatusIsDifferent_ShouldRaiseEventWithCorrectData()
        {
            // Arrange
            var resume = CreateResumeInstance();
            var expectedPrev = ResumeStatus.Applied;
            var expectedCurrent = ResumeStatus.Offer;

            // Act
            resume.UpdateStatus(expectedCurrent);

            // Assert
            var raisedEvent = resume.DomainEvents
                .OfType<ResumeStatusChangedEvent>()
                .Single();

            raisedEvent.PreviousStatus.Should().Be(expectedPrev);
            raisedEvent.CurrentStatus.Should().Be(expectedCurrent);
            raisedEvent.ResumeId.Should().Be(resume.Id);
        }

        // ─── Test 4: Same status → no change, no event ───────────────────
        [Fact]
        public void UpdateStatus_WhenStatusIsSame_ShouldNotRaiseEvent()
        {
            // Arrange
            var resume = CreateResumeInstance();
            // Act
            resume.UpdateStatus(ResumeStatus.Applied);  // same status

            // Assert
            resume.DomainEvents.Should().BeEmpty();
            resume.ResumeStatus.Should().Be(ResumeStatus.Applied);
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
}