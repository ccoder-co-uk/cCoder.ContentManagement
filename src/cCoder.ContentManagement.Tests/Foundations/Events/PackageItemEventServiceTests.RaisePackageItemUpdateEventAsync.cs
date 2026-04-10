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

public partial class PackageItemEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePackageItemUpdateEventAsync()
    {
        // Given
        PackageItem entity = new();
        EventMessage<cCoder.Data.Models.Packaging.PackageItem> actualMessage = null;

        packageItemEventBrokerMock
            .Setup(x => x.RaisePackageItemUpdateEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Packaging.PackageItem>>()))
            .Callback<EventMessage<cCoder.Data.Models.Packaging.PackageItem>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePackageItemUpdateEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        packageItemEventBrokerMock.Verify(
            x => x.RaisePackageItemUpdateEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Packaging.PackageItem>>()),
            Times.Once
        );
        packageItemEventBrokerMock.VerifyNoOtherCalls();
    }

}















