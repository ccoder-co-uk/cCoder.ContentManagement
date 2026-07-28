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

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class CommonObjectEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseCommonObjectUpdateEventAsync()
    {
        // Given
        CommonObject entity = new();
        EventMessage<CommonObject> actualMessage = null;

        commonObjectEventBrokerMock
            .Setup(expression: x => x.RaiseCommonObjectUpdateEventAsync(message: It.IsAny<EventMessage<CommonObject>>()))
            .Callback<EventMessage<CommonObject>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCommonObjectUpdateEventAsync(entity: entity);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeSameAs(expected: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        commonObjectEventBrokerMock.Verify(
expression: x => x.RaiseCommonObjectUpdateEventAsync(message: It.IsAny<EventMessage<CommonObject>>()),
times: Times.Once
        );

        commonObjectEventBrokerMock.Verify(expression: x => x.GetCurrentUserId(), times: Times.Once);


        commonObjectEventBrokerMock.VerifyNoOtherCalls();
    }

}