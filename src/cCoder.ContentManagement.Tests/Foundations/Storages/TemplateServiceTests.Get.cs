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

public partial class TemplateServiceTests
{
    [Fact]
    public void ShouldReturnTemplateWhenGet()
    {
        // Given
        Template template = CreateRandomTemplate(id: 5);

        templateBrokerMock.Setup(x => x.GetAllTemplates(false)).Returns(new[] { template }.AsQueryable());

        // When
        Template result = templateService.Get(5);

        // Then
        result.Should().BeEquivalentTo(template);
        templateBrokerMock.Verify(x => x.GetAllTemplates(false), Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}


















