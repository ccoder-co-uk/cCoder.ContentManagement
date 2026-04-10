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

public partial class PageInfoEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePageInfoAddEventAsync()
    {
        // Given
        PageInfo entity = new();
        EventMessage<PageInfo> actualMessage = null;

        pageInfoEventBrokerMock
            .Setup(x => x.RaisePageInfoAddEventAsync(It.IsAny<EventMessage<PageInfo>>()))
            .Callback<EventMessage<PageInfo>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePageInfoAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeSameAs(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        pageInfoEventBrokerMock.Verify(
            x => x.RaisePageInfoAddEventAsync(It.IsAny<EventMessage<PageInfo>>()),
            Times.Once
        );
        pageInfoEventBrokerMock.VerifyNoOtherCalls();
    }

}


















