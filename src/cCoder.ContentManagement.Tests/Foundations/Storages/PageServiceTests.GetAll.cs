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
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    [Fact]
    public void ShouldReturnPagesWhenGetAll()
    {
        // Given
        Page[] expectedItems = [CreateRandomPage(id: 1)];

        IQueryable<CmsDataModels.Page> pages = expectedItems.Select(selector: item => item)
            .AsQueryable();

        pageBrokerMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: pages);

        // When
        IQueryable<Page> result = pageService.GetAllPage();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        pageBrokerMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}