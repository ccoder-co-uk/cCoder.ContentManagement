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
    public async Task ShouldMapAndCallBrokerWhenRaiseLayoutAddEventAsync()
    {
        // Given
        Layout entity = new();
        EventMessage<CmsDataModels.Layout> actualMessage = null;

        layoutEventBrokerMock
            .Setup(x => x.RaiseLayoutAddEventAsync(It.IsAny<EventMessage<CmsDataModels.Layout>>()))
            .Callback<EventMessage<CmsDataModels.Layout>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        layoutEventBrokerMock.Verify(
            x => x.RaiseLayoutAddEventAsync(It.IsAny<EventMessage<CmsDataModels.Layout>>()),
            Times.Once
        );
        layoutEventBrokerMock.VerifyNoOtherCalls();
    }

}

















