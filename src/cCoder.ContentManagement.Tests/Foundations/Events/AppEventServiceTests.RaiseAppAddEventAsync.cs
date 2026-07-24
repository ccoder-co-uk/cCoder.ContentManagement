// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
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

public partial class AppEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseAppAddEventAsync()
    {
        // Given
        App entity = new();
        EventMessage<CmsDataModels.App> actualMessage = null;

        appEventBrokerMock
            .Setup(expression: x => x.RaiseAppAddEventAsync(message: It.IsAny<EventMessage<CmsDataModels.App>>()))
            .Callback<EventMessage<CmsDataModels.App>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseAppAddEventAsync(app: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        appEventBrokerMock.Verify(
expression: x => x.RaiseAppAddEventAsync(message: It.IsAny<EventMessage<CmsDataModels.App>>()),
times: Times.Once
        );

        appEventBrokerMock.Verify(expression: x => x.GetCurrentUserId(), times: Times.Once);


        appEventBrokerMock.VerifyNoOtherCalls();
    }

}