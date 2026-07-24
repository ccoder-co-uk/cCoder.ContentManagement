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

public partial class LayoutEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseLayoutDeleteEventAsync()
    {
        // Given
        Layout entity = new();
        EventMessage<CmsDataModels.Layout> actualMessage = null;

        layoutEventBrokerMock
            .Setup(expression: x => x.RaiseLayoutDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Layout>>()))
            .Callback<EventMessage<CmsDataModels.Layout>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutDeleteEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        layoutEventBrokerMock.Verify(
expression: x => x.RaiseLayoutDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Layout>>()),
times: Times.Once
        );

        layoutEventBrokerMock.VerifyNoOtherCalls();
    }

}