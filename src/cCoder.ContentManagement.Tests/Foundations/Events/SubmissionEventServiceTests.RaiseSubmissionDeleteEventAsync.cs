// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class SubmissionEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseSubmissionDeleteEventAsync()
    {
        // Given
        Submission entity = new() { DataJson = "{}" };
        EventMessage<CmsDataModels.Submission> actualMessage = null;

        submissionEventBrokerMock
            .Setup(expression: x => x.RaiseSubmissionDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Submission>>()))
            .Callback<EventMessage<CmsDataModels.Submission>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseSubmissionDeleteEventAsync(
            submission: entity,
            userId: CurrentUserId);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        submissionEventBrokerMock.Verify(
expression: x => x.RaiseSubmissionDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Submission>>()),
times: Times.Once
        );

        submissionEventBrokerMock.VerifyNoOtherCalls();
    }

}