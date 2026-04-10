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

public partial class CultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Culture[] expectedItems = [CreateRandomCulture()];
        IQueryable<CmsDataModels.Culture> cultures = expectedItems
            .Select(item => item)
            .AsQueryable();

        cultureBrokerMock.Setup(x => x.GetAllCultures(false)).Returns(cultures);

        // When
        IQueryable<Culture> result = cultureService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        cultureBrokerMock.Verify(x => x.GetAllCultures(false), Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
        appCultureBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















