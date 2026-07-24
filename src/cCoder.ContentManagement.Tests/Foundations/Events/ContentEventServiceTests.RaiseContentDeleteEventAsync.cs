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

public partial class ContentEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseContentDeleteEventAsync()
    {
        // Given
        Content entity = new();
        EventMessage<CmsDataModels.Content> actualMessage = null;

        contentEventBrokerMock
            .Setup(expression: x => x.RaiseContentDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Content>>()))
            .Callback<EventMessage<CmsDataModels.Content>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseContentDeleteEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        contentEventBrokerMock.Verify(
expression: x => x.RaiseContentDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.Content>>()),
times: Times.Once
        );

        contentEventBrokerMock.Verify(expression: x => x.GetCurrentUserId(), times: Times.Once);


        contentEventBrokerMock.VerifyNoOtherCalls();
    }

}