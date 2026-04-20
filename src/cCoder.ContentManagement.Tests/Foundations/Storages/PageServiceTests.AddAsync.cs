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

public partial class PageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        Page page = CreateRandomPage(id: 0);

        CmsDataModels.Page submitted = null;


        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Page_create"));

        pageBrokerMock
            .Setup(x => x.AddPageAsync(It.Is<CmsDataModels.Page>(candidate => !ReferenceEquals(candidate, page))))
            .Callback<CmsDataModels.Page>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Page value) => value);

        // When
        Page result = await pageService.AddAsync(page);

        // Then
        result.Should().NotBeSameAs(page);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(
                page,
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
                        .Excluding(candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
                page,
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
                        .Excluding(candidate => candidate.Id)
            );

        pageBrokerMock.Verify(
            x => x.AddPageAsync(It.Is<CmsDataModels.Page>(candidate => !ReferenceEquals(candidate, page))),
            Times.Once
        );
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Page_create"), Times.Once);
    }

    [Fact]
    public async Task ShouldPreserveShowOnMenusWhenAddingHiddenPageAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        Page page = CreateRandomPage(id: 0);
        page.ShowOnMenus = false;

        CmsDataModels.Page submitted = null;

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Page_create"));

        pageBrokerMock
            .Setup(x => x.AddPageAsync(It.Is<CmsDataModels.Page>(candidate => !ReferenceEquals(candidate, page))))
            .Callback<CmsDataModels.Page>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Page value) => value);

        // When
        Page result = await pageService.AddAsync(page);

        // Then
        submitted.Should().NotBeNull();
        submitted.ShowOnMenus.Should().BeFalse();
        result.ShowOnMenus.Should().BeFalse();

        pageBrokerMock.Verify(
            x => x.AddPageAsync(It.Is<CmsDataModels.Page>(candidate => !ReferenceEquals(candidate, page))),
            Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Page_create"), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        Page page = CreateRandomPage(id: 0);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Page_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await pageService.AddAsync(page);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Page_create"), Times.Once);
    }

}















