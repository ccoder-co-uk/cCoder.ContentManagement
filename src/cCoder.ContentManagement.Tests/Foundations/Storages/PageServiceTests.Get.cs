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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    [Fact]
    public void ShouldReturnPageWhenGet()
    {
        // Given
        Page page = CreateRandomPage(id: 5);

        pageBrokerMock.Setup(x => x.GetAllPages(false)).Returns(new[] { page }.AsQueryable());

        // When
        Page result = pageService.Get(5);

        // Then
        result.Should().BeEquivalentTo(page);
        pageBrokerMock.Verify(x => x.GetAllPages(false), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}


















