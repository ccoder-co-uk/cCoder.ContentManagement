// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
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

        foreach (PageInfo pageInfo in page.PageInfo)
        {
            pageInfo.Id = 0;
        }

        foreach (Content content in page.Contents)
        {
            content.Id = 0;
        }

        LocalPageInfo[] localPageInfos = ToLocalPageInfos(pageInfos: page.PageInfo);

        LocalContent[] localContents = [.. page.Contents.Select(selector: content => new LocalContent
        {
            Id = content.Id,
            PageId = page.Id,
            Name = content.Name,
            CultureId = content.CultureId,
            Html = content.Html,
        })];

        LocalPageRole[] localPageRoles = [.. page.Roles.Select(selector: role => new LocalPageRole
        {
            PageId = page.Id,
            RoleId = role.RoleId,
        })];

        foreach (LocalPageInfo pageInfo in localPageInfos)
        {
            pageInfoOrchestrationServiceMock
                .Setup(expression: service => service.AddPageInfoAsync(
                    newPageInfo: It.Is<LocalPageInfo>(match: item =>
                        item.Id == pageInfo.Id && item.PageId == page.Id)))
                .ReturnsAsync(value: pageInfo);
        }

        foreach (LocalContent content in localContents)
        {
            contentOrchestrationServiceMock
                .Setup(expression: service => service.AddContentAsync(
                    newContent: It.Is<LocalContent>(match: item =>
                        item.Id == content.Id && item.PageId == page.Id)))
                .ReturnsAsync(value: content);
        }

        pageRoleOrchestrationServiceMock
            .Setup(expression: service => service.AddOrUpdatePageRoleResult(
newPageRole: It.Is<IEnumerable<LocalPageRole>>(match: items =>
                    items.Select(selector: item => item.RoleId)
            .SequenceEqual(second: localPageRoles.Select(selector: item => item.RoleId))
                )
            ))
            .ReturnsAsync(value: []);

        // When
        await coordinationService.HandlePageAddAsync(page: page);
        await structureCoordinationService.HandlePageAddAsync(page: page);

        // Then

        foreach (LocalPageInfo pageInfo in localPageInfos)
        {
            pageInfoOrchestrationServiceMock.Verify(
                expression: service => service.AddPageInfoAsync(
                    newPageInfo: It.Is<LocalPageInfo>(match: item =>
                        item.Id == pageInfo.Id && item.PageId == page.Id)),
                times: Times.Once);
        }

        foreach (LocalContent content in localContents)
        {
            contentOrchestrationServiceMock.Verify(
                expression: service => service.AddContentAsync(
                    newContent: It.Is<LocalContent>(match: item =>
                        item.Id == content.Id && item.PageId == page.Id)),
                times: Times.Once);
        }

        pageRoleOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdatePageRoleResult(
newPageRole: It.Is<IEnumerable<LocalPageRole>>(match: items =>
                    items.Select(selector: item => item.RoleId)
            .SequenceEqual(second: localPageRoles.Select(selector: item => item.RoleId))
                )
            ),
times: Times.Once
        );

        pageInfoOrchestrationServiceMock.VerifyNoOtherCalls();
        contentOrchestrationServiceMock.VerifyNoOtherCalls();
        pageRoleOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}