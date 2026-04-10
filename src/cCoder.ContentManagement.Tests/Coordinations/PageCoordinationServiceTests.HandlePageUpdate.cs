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
using Moq;
using Xunit;
using LocalContent = cCoder.Data.Models.CMS.Content;
using LocalPage = cCoder.Data.Models.CMS.Page;
using LocalPageInfo = cCoder.Data.Models.CMS.PageInfo;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;


namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageCoordinationServiceTests
{
    [Fact]
    public async Task ShouldAddOrUpdateChildCollectionsWhenHandlePageUpdate()
    {
        // Given
        Page page = CreateRandomPage();
        page.Id = 42;
        page.PageInfo =
        [
            new PageInfo
            {
                Id = 0,
                PageId = page.Id,
                CultureId = string.Empty,
                Title = "Updated title",
                Description = "Updated description",
                Keywords = "Updated keywords"
            }
        ];
        page.Contents =
        [
            new Content
            {
                Id = 0,
                PageId = page.Id,
                Name = "Body",
                CultureId = string.Empty,
                Html = "Updated html"
            }
        ];
        page.Roles =
        [
            new PageRole
            {
                PageId = page.Id,
                RoleId = Guid.NewGuid()
            }
        ];

        LocalPageInfo[] existingPageInfos =
        [
            new LocalPageInfo
            {
                Id = 100,
                PageId = page.Id,
                CultureId = string.Empty,
                Title = "Old title",
                Description = "Old description",
                Keywords = "Old keywords"
            }
        ];
        LocalContent[] existingContents =
        [
            new LocalContent
            {
                Id = 200,
                PageId = page.Id,
                Name = "Body",
                CultureId = string.Empty,
                Html = "Old html",
            }
        ];
        LocalPageRole[] existingPageRoles =
        [
            new LocalPageRole
            {
                PageId = page.Id,
                RoleId = page.Roles.Single().RoleId
            }
        ];
        page.Pages = [];
        Page existingChild = new() { Id = 84, ParentId = page.Id };
        LocalPage localExistingChild = new()
        {
            Id = existingChild.Id,
            ParentId = existingChild.ParentId,
            AppId = existingChild.AppId,
            Name = existingChild.Name,
            Path = existingChild.Path,
            Layout = existingChild.Layout,
            ResourceKey = existingChild.ResourceKey,
            Order = existingChild.Order,
            ShowOnMenus = existingChild.ShowOnMenus,
            CreatedBy = existingChild.CreatedBy,
            CreatedOn = existingChild.CreatedOn,
            LastUpdated = existingChild.LastUpdated,
            LastUpdatedBy = existingChild.LastUpdatedBy,
        };

        pageInfoOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(existingPageInfos.AsQueryable());

        pageInfoOrchestrationServiceMock
            .Setup(service =>
                service.UpdateAsync(It.Is<LocalPageInfo>(item =>
                    item.Id == existingPageInfos.Single().Id &&
                    item.PageId == page.Id &&
                    item.CultureId == string.Empty &&
                    item.Title == page.PageInfo.Single().Title &&
                    item.Description == page.PageInfo.Single().Description &&
                    item.Keywords == page.PageInfo.Single().Keywords)))
            .ReturnsAsync((LocalPageInfo item) => item);

        contentOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(existingContents.AsQueryable());

        contentOrchestrationServiceMock
            .Setup(service => service.UpdateAsync(It.Is<LocalContent>(item =>
                item.Id == existingContents.Single().Id &&
                item.PageId == page.Id &&
                item.Name == page.Contents.Single().Name &&
                item.CultureId == page.Contents.Single().CultureId &&
                item.Html == page.Contents.Single().Html)))
            .ReturnsAsync((LocalContent item) => item);

        pageRoleOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(existingPageRoles.AsQueryable());

        pageOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[] { localExistingChild }.AsQueryable());

        pageOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(It.Is<IEnumerable<LocalPage>>(pages => pages.Single().Id == existingChild.Id)))
            .ReturnsAsync([]);

        // When
        await coordinationService.HandlePageUpdateAsync(page);

        // Then
        pageInfoOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        pageInfoOrchestrationServiceMock.Verify(service =>
            service.UpdateAsync(It.Is<LocalPageInfo>(item =>
                item.Id == existingPageInfos.Single().Id &&
                item.PageId == page.Id &&
                item.CultureId == string.Empty &&
                item.Title == page.PageInfo.Single().Title &&
                item.Description == page.PageInfo.Single().Description &&
                item.Keywords == page.PageInfo.Single().Keywords)),
            Times.Once);

        contentOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        contentOrchestrationServiceMock.Verify(service =>
            service.UpdateAsync(It.Is<LocalContent>(item =>
                item.Id == existingContents.Single().Id &&
                item.PageId == page.Id &&
                item.Name == page.Contents.Single().Name &&
                item.CultureId == page.Contents.Single().CultureId &&
                item.Html == page.Contents.Single().Html)),
            Times.Once);

        pageInfoOrchestrationServiceMock.VerifyNoOtherCalls();
        contentOrchestrationServiceMock.VerifyNoOtherCalls();
        pageRoleOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        pageRoleOrchestrationServiceMock.VerifyNoOtherCalls();
        pageOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        pageOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<LocalPage>>(pages => pages.Single().Id == existingChild.Id)),
            Times.Once
        );
        pageOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}





















