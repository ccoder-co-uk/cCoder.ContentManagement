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
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class CommonObjectEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseCommonObjectDeleteEventAsync()
    {
        // Given
        CommonObject entity = new();
        EventMessage<CommonObject> actualMessage = null;

        commonObjectEventBrokerMock
            .Setup(x => x.RaiseCommonObjectDeleteEventAsync(It.IsAny<EventMessage<CommonObject>>()))
            .Callback<EventMessage<CommonObject>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseCommonObjectDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeSameAs(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        commonObjectEventBrokerMock.Verify(
            x => x.RaiseCommonObjectDeleteEventAsync(It.IsAny<EventMessage<CommonObject>>()),
            Times.Once
        );
        commonObjectEventBrokerMock.VerifyNoOtherCalls();
    }

}
















