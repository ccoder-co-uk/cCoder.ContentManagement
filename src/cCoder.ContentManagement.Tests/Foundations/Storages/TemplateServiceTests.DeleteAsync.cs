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
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class TemplateServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Template template = CreateRandomTemplate(id: 5);

        templateBrokerMock.Setup(expression: x => x.GetAllTemplates())
            .Returns(value: new[] { template }.AsQueryable());

        templateBrokerMock.Setup(expression: x => x.DeleteTemplateAsync(deletedTemplate: It.IsAny<CmsDataModels.Template>()))
            .ReturnsAsync(value: 1);

        // When
        await templateService.DeleteAsync(templateId: 5);

        // Then
        templateBrokerMock.Verify(expression: x => x.GetAllTemplates(), times: Times.Once);
        templateBrokerMock.Verify(expression: x => x.DeleteTemplateAsync(deletedTemplate: It.Is<CmsDataModels.Template>(match: actual => actual.Id == template.Id)), times: Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
    }

}
