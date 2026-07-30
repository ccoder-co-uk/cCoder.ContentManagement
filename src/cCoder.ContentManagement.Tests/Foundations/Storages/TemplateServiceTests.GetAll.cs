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

public partial class TemplateServiceTests
{
    [Fact]
    public void ShouldReturnTemplatesWhenGetAll()
    {
        // Given
        Template[] expectedItems = [CreateRandomTemplate(id: 1)];

        IQueryable<CmsDataModels.Template> templates = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        templateBrokerMock.Setup(expression: x => x.GetAllTemplates())
            .Returns(value: templates);

        // When
        IQueryable<Template> result = templateService.GetAllTemplate();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        templateBrokerMock.Verify(expression: x => x.GetAllTemplates(), times: Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}