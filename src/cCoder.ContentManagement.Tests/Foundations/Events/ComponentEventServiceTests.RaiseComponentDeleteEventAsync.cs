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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class ComponentEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseComponentDeleteEventAsync()
    {
        // Given
        Component entity = new();
        EventMessage<CmsDataModels.Component> actualMessage = null;

        componentEventBrokerMock
            .Setup(x => x.RaiseComponentDeleteEventAsync(It.IsAny<EventMessage<CmsDataModels.Component>>()))
            .Callback<EventMessage<CmsDataModels.Component>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseComponentDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        componentEventBrokerMock.Verify(
            x => x.RaiseComponentDeleteEventAsync(It.IsAny<EventMessage<CmsDataModels.Component>>()),
            Times.Once
        );
        componentEventBrokerMock.VerifyNoOtherCalls();
    }

}

















