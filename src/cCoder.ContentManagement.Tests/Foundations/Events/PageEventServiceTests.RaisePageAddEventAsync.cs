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

public partial class PageEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePageAddEventAsync()
    {
        // Given
        Page entity = new();
        EventMessage<CmsDataModels.Page> actualMessage = null;

        pageEventBrokerMock
            .Setup(x => x.RaisePageAddEventAsync(It.IsAny<EventMessage<CmsDataModels.Page>>()))
            .Callback<EventMessage<CmsDataModels.Page>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePageAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        pageEventBrokerMock.Verify(
            x => x.RaisePageAddEventAsync(It.IsAny<EventMessage<CmsDataModels.Page>>()),
            Times.Once
        );
        pageEventBrokerMock.VerifyNoOtherCalls();
    }

}

















