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

public partial class TemplateServiceTests
{
    [Fact]
    public void ShouldReturnTemplatesWhenGetAll()
    {
        // Given
        Template[] expectedItems = [CreateRandomTemplate(id: 1)];
        IQueryable<CmsDataModels.Template> templates = expectedItems
            .Select(item => item)
            .AsQueryable();

        templateBrokerMock.Setup(x => x.GetAllTemplates(false)).Returns(templates);

        // When
        IQueryable<Template> result = templateService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectedItems);
        templateBrokerMock.Verify(x => x.GetAllTemplates(false), Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















