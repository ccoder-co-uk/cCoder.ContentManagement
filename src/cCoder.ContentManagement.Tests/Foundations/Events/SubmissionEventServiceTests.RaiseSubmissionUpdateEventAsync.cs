// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class SubmissionEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseSubmissionUpdateEventAsync()
    {
        // Given
        Submission entity = new() { DataJson = "{}" };
        EventMessage<CmsDataModels.Submission> actualMessage = null;

        submissionEventBrokerMock
            .Setup(expression: x => x.RaiseSubmissionUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Submission>>()))
            .Callback<EventMessage<CmsDataModels.Submission>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseSubmissionUpdateEventAsync(entity: entity);

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
expression: x => x.RaiseSubmissionUpdateEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Submission>>()),
times: Times.Once
        );

        submissionEventBrokerMock.Verify(expression: x => x.GetCurrentUserId(), times: Times.Once);


        submissionEventBrokerMock.VerifyNoOtherCalls();
    }

}