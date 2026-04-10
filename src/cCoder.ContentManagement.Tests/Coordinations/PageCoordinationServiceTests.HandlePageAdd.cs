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
using LocalPageInfo = cCoder.Data.Models.CMS.PageInfo;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;


namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageCoordinationServiceTests
{
    [Fact]
    public async Task ShouldAddOrUpdateChildCollectionsWhenHandlePageAdd()
    {
        // Given
        Page page = CreateRandomPage();
        LocalPageInfo[] localPageInfos = ToLocalPageInfos(page.PageInfo);
        LocalContent[] localContents = [.. page.Contents.Select(content => new LocalContent
        {
            Id = content.Id,
            PageId = page.Id,
            Name = content.Name,
            CultureId = content.CultureId,
            Html = content.Html,
        })];
        LocalPageRole[] localPageRoles = [.. page.Roles.Select(role => new LocalPageRole
        {
            PageId = page.Id,
            RoleId = role.RoleId,
        })];

        pageInfoOrchestrationServiceMock
            .Setup(service =>
                service.AddOrUpdate(
                    It.Is<IEnumerable<LocalPageInfo>>(items =>
                        items.Select(i => i.Id).SequenceEqual(localPageInfos.Select(i => i.Id))
                    )
                )
            )
            .ReturnsAsync([]);

        contentOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(
                It.Is<IEnumerable<LocalContent>>(items =>
                    items.Select(item => item.PageId).SequenceEqual(localContents.Select(item => item.PageId))
                )
            ))
            .ReturnsAsync([]);

        pageRoleOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(
                It.Is<IEnumerable<LocalPageRole>>(items =>
                    items.Select(item => item.RoleId).SequenceEqual(localPageRoles.Select(item => item.RoleId))
                )
            ))
            .ReturnsAsync([]);

        // When
        await coordinationService.HandlePageAddAsync(page);

        // Then
        pageInfoOrchestrationServiceMock.Verify(
            service =>
                service.AddOrUpdate(
                    It.Is<IEnumerable<LocalPageInfo>>(items =>
                        items.Select(i => i.Id).SequenceEqual(localPageInfos.Select(i => i.Id))
                    )
                ),
            Times.Once
        );

        contentOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(
                It.Is<IEnumerable<LocalContent>>(items =>
                    items.Select(item => item.PageId).SequenceEqual(localContents.Select(item => item.PageId))
                )
            ),
            Times.Once
        );

        pageRoleOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(
                It.Is<IEnumerable<LocalPageRole>>(items =>
                    items.Select(item => item.RoleId).SequenceEqual(localPageRoles.Select(item => item.RoleId))
                )
            ),
            Times.Once
        );

        pageInfoOrchestrationServiceMock.VerifyNoOtherCalls();
        contentOrchestrationServiceMock.VerifyNoOtherCalls();
        pageRoleOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}





















