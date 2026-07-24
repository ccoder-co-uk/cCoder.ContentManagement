// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public partial class AppServiceTests
{
    [Fact]
    public void ShouldReturnAppsWhenGetAll()
    {
        // Given
        App[] expectedApps = [CreateRandomApp(id: 1)];

        IQueryable<CmsDataModels.App> apps = expectedApps.Select(selector: app => app)
            .AsQueryable();

        appBrokerMock.Setup(expression: x => x.GetAllApps(ignoreFilters: false))
            .Returns(value: apps);

        // When
        IQueryable<App> result = appService.GetAllApp();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedApps);

        appBrokerMock.Verify(expression: x => x.GetAllApps(ignoreFilters: false), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}