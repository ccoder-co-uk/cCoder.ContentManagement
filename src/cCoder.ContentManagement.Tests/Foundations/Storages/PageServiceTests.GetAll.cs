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
        IQueryable<CmsDataModels.Page> pages = expectedItems.Select(item => item).AsQueryable();

        pageBrokerMock.Setup(x => x.GetAllPages(false)).Returns(pages);

        // When
        IQueryable<Page> result = pageService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        pageBrokerMock.Verify(x => x.GetAllPages(false), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}


















