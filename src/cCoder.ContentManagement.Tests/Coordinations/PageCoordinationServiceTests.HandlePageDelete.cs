// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        PageInfo pageInfo = Builder<PageInfo>.CreateNew()
            .With(func: item => item.PageId = page.Id)
            .Build();

        Content content = Builder<Content>.CreateNew()
            .With(func: item => item.PageId = page.Id)
            .Build();

        PageRole pageRole = Builder<PageRole>.CreateNew()
            .With(func: item => item.PageId = page.Id)
            .Build();

        LocalPageInfo[] localPageInfos = ToLocalPageInfos(pageInfos: [pageInfo]);

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

        pageRoleOrchestrationServiceMock.Setup(expression: service => service.GetAllPageRole(ignoreFilters: true))
            .Returns(value: pageRoles);

        pageInfoOrchestrationServiceMock.Setup(expression: service => service.GetAllPageInfo(ignoreFilters: true))
            .Returns(value: pageInfos);

        contentOrchestrationServiceMock.Setup(expression: service => service.GetAllContent(ignoreFilters: true))
            .Returns(value: contents);

        pageRoleOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllPageRoleAsync(deletedPageRole: It.IsAny<IEnumerable<LocalPageRole>>()))
            .Returns(value: ValueTask.CompletedTask);

        pageInfoOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllPageInfoAsync(deletedPageInfo: It.IsAny<IEnumerable<LocalPageInfo>>()))
            .Returns(value: ValueTask.CompletedTask);

        contentOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllContentAsync(deletedContent: It.IsAny<IEnumerable<LocalContent>>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.HandlePageDeleteAsync(page: page);
        await structureCoordinationService.HandlePageDeleteAsync(page: page);

        // Then
        pageRoleOrchestrationServiceMock.Verify(expression: service => service.GetAllPageRole(ignoreFilters: true), times: Times.Once);
        pageInfoOrchestrationServiceMock.Verify(expression: service => service.GetAllPageInfo(ignoreFilters: true), times: Times.Once);
        contentOrchestrationServiceMock.Verify(expression: service => service.GetAllContent(ignoreFilters: true), times: Times.Once);

        pageRoleOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllPageRoleAsync(
deletedPageRole: It.Is<IEnumerable<LocalPageRole>>(match: items => items.Single()
            .PageId == page.Id)
            ),
times: Times.Once
        );

        pageInfoOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllPageInfoAsync(
deletedPageInfo: It.Is<IEnumerable<LocalPageInfo>>(match: items => items.Single()
            .PageId == page.Id)
            ),
times: Times.Once
        );

        contentOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllContentAsync(
deletedContent: It.Is<IEnumerable<LocalContent>>(match: items => items.Single()
            .PageId == page.Id)
            ),
times: Times.Once
        );

        pageRoleOrchestrationServiceMock.VerifyNoOtherCalls();
        pageInfoOrchestrationServiceMock.VerifyNoOtherCalls();
        contentOrchestrationServiceMock.VerifyNoOtherCalls();
    }

}