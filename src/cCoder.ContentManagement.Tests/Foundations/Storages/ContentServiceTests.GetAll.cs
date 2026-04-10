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

public partial class ContentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Content[] expectedItems = [CreateRandomContent()];
        IQueryable<CmsDataModels.Content> contents = expectedItems
            .Select(item => item)
            .AsQueryable();

        contentBrokerMock.Setup(x => x.GetAllContents(false)).Returns(contents);

        // When
        IQueryable<Content> result = contentService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        contentBrokerMock.Verify(x => x.GetAllContents(false), Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















