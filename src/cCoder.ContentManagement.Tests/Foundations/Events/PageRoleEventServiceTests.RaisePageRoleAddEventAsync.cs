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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageRoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePageRoleAddEventAsync()
    {
        // Given
        PageRole entity = new();
        EventMessage<PageRole> actualMessage = null;

        pageRoleEventBrokerMock
            .Setup(expression: x => x.RaisePageRoleAddEventAsync(message: It.IsAny<EventMessage<PageRole>>()))
            .Callback<EventMessage<PageRole>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageRoleAddEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeSameAs(expected: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        pageRoleEventBrokerMock.Verify(
expression: x => x.RaisePageRoleAddEventAsync(message: It.IsAny<EventMessage<PageRole>>()),
times: Times.Once
        );

        pageRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}