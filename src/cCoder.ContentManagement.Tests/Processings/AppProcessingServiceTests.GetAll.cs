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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<App> entities = new[] { CreateRandomApp() }.AsQueryable();

        appServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: entities);

        // When
        IQueryable<App> result = appProcessingService.GetAllApp();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        appServiceMock.Verify(expression: x => x.GetAllApp(), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}