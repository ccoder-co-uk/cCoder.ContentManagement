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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;
using SecurityDataModels = cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public async Task ShouldDelegateToDataContextWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SecurityDataModels.User { Id = "test-user" });

        Resource resource = CreateRandomResource(id: 5, appId: 7);
        CmsDataModels.Resource submitted = null;
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Resource_update"));

        resourceBrokerMock
            .Setup(x => x.UpdateResourceAsync(It.IsAny<CmsDataModels.Resource>()))
            .Callback<CmsDataModels.Resource>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Resource value) => value);

        // When
        Resource result = await resourceService.UpdateAsync(resource);

        // Then
        result.Should().NotBeSameAs(resource);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(
                resource,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
            );

        result
            .Should()
            .BeEquivalentTo(
                resource,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
            );

        resourceBrokerMock.Verify(x => x.UpdateResourceAsync(It.IsAny<CmsDataModels.Resource>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SecurityDataModels.User { Id = "test-user" });

        Resource resource = CreateRandomResource(id: 5, appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Resource_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await resourceService.UpdateAsync(resource)
        );

        // Then
    }

}
















