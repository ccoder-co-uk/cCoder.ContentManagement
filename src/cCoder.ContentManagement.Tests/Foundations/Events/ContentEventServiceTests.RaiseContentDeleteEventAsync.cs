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
            .Setup(x => x.RaiseContentDeleteEventAsync(It.IsAny<EventMessage<CmsDataModels.Content>>()))
            .Callback<EventMessage<CmsDataModels.Content>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseContentDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        contentEventBrokerMock.Verify(
            x => x.RaiseContentDeleteEventAsync(It.IsAny<EventMessage<CmsDataModels.Content>>()),
            Times.Once
        );
        contentEventBrokerMock.VerifyNoOtherCalls();
    }

}

















