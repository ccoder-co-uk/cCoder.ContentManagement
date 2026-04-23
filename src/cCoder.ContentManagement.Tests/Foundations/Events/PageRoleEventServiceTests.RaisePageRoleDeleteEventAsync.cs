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
    public async Task ShouldMapAndCallBrokerWhenRaisePageRoleDeleteEventAsync()
    {
        // Given
        PageRole entity = new();
        EventMessage<PageRole> actualMessage = null;

        pageRoleEventBrokerMock
            .Setup(x => x.RaisePageRoleDeleteEventAsync(It.IsAny<EventMessage<PageRole>>()))
            .Callback<EventMessage<PageRole>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePageRoleDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeSameAs(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        pageRoleEventBrokerMock.Verify(
            x => x.RaisePageRoleDeleteEventAsync(It.IsAny<EventMessage<PageRole>>()),
            Times.Once
        );
        pageRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}


















