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
using FizzWare.NBuilder;
using Moq;
using Xunit;
using LocalContent = cCoder.Data.Models.CMS.Content;
using LocalPageInfo = cCoder.Data.Models.CMS.PageInfo;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;


namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageCoordinationServiceTests
{
    [Fact]
    public async Task ShouldFetchAndDeleteChildCollectionsWhenHandlePageDelete()
    {
        // Given
        Page page = CreateRandomPage();

        PageInfo pageInfo = Builder<PageInfo>.CreateNew().With(item => item.PageId = page.Id).Build();
        Content content = Builder<Content>.CreateNew().With(item => item.PageId = page.Id).Build();
        PageRole pageRole = Builder<PageRole>.CreateNew().With(item => item.PageId = page.Id).Build();

        LocalPageInfo[] localPageInfos = ToLocalPageInfos([pageInfo]);
        IQueryable<LocalPageRole> pageRoles = new[]
        {
            new LocalPageRole
            {
                PageId = pageRole.PageId,
                RoleId = pageRole.RoleId
            }
        }.AsQueryable();
        IQueryable<LocalContent> contents = new[]
        {
            new LocalContent
            {
                Id = content.Id,
                PageId = content.PageId,
                Name = content.Name,
                CultureId = content.CultureId,
                Html = content.Html,
            }
        }.AsQueryable();
        IQueryable<LocalPageInfo> pageInfos = localPageInfos.AsQueryable();

        pageRoleOrchestrationServiceMock.Setup(service => service.GetAll(true)).Returns(pageRoles);
        pageInfoOrchestrationServiceMock.Setup(service => service.GetAll(true)).Returns(pageInfos);
        contentOrchestrationServiceMock.Setup(service => service.GetAll(true)).Returns(contents);

        pageRoleOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(It.IsAny<IEnumerable<LocalPageRole>>()))
            .Returns(ValueTask.CompletedTask);

        pageInfoOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(It.IsAny<IEnumerable<LocalPageInfo>>()))
            .Returns(ValueTask.CompletedTask);

        contentOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(It.IsAny<IEnumerable<LocalContent>>()))
            .Returns(ValueTask.CompletedTask);

        // When
        await coordinationService.HandlePageDeleteAsync(page);

        // Then
        pageRoleOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        pageInfoOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        contentOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);

        pageRoleOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(
                It.Is<IEnumerable<LocalPageRole>>(items => items.Single().PageId == page.Id)
            ),
            Times.Once
        );

        pageInfoOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(
                It.Is<IEnumerable<LocalPageInfo>>(items => items.Single().PageId == page.Id)
            ),
            Times.Once
        );

        contentOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(
                It.Is<IEnumerable<LocalContent>>(items => items.Single().PageId == page.Id)
            ),
            Times.Once
        );

        pageRoleOrchestrationServiceMock.VerifyNoOtherCalls();
        pageInfoOrchestrationServiceMock.VerifyNoOtherCalls();
        contentOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}





















