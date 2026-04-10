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

public partial class AppCultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        AppCulture[] expectedItems = [CreateRandomAppCulture()];
        IQueryable<CmsDataModels.AppCulture> appCultures = expectedItems
            .Select(item => item)
            .AsQueryable();

        appCultureBrokerMock.Setup(x => x.GetAllAppCultures(false)).Returns(appCultures);

        // When
        IQueryable<AppCulture> result = appCultureService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        appCultureBrokerMock.Verify(x => x.GetAllAppCultures(false), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















