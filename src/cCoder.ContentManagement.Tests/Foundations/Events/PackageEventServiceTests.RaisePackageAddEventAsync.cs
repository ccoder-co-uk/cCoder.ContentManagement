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

public partial class PackageEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePackageAddEventAsync()
    {
        // Given
        Package entity = new();
        EventMessage<cCoder.Data.Models.Packaging.Package> actualMessage = null;

        packageEventBrokerMock
            .Setup(x => x.RaisePackageAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Packaging.Package>>()))
            .Callback<EventMessage<cCoder.Data.Models.Packaging.Package>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePackageAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        packageEventBrokerMock.Verify(
            x => x.RaisePackageAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Packaging.Package>>()),
            Times.Once
        );
        packageEventBrokerMock.VerifyNoOtherCalls();
    }

}















