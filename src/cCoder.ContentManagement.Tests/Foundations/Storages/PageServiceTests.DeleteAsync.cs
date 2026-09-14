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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenPageExistsForDeleteAsync()
    {
        // Given
        Page page = CreateRandomPage(id: 5);

        pageBrokerMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { page }.AsQueryable());

        pageBrokerMock
            .Setup(expression: x => x.DeletePageAsync(deletedPage: It.Is<CmsDataModels.Page>(match: p => p.Id == page.Id)))
            .ReturnsAsync(value: 1);

        // When
        await pageService.DeleteAsync(pageId: 5);

        // Then
        pageBrokerMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageBrokerMock.Verify(expression: x => x.DeletePageAsync(deletedPage: It.Is<CmsDataModels.Page>(match: p => p.Id == page.Id)), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
    }

}