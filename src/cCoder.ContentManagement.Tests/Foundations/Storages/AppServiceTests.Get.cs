// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.ContentManagement.Models;
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
using System.Security;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    [Fact]
    public void ShouldReturnAppWhenGet()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        appBrokerMock.Setup(expression: x => x.GetAllApps())
            .Returns(value: new[] { app }.AsQueryable());

        // When
        App result = appService.GetVisibleAppsAppOperation(appOperation: new AppOperation()).Apps.Single(predicate: app => app.Id == 5);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: app);

        appBrokerMock.Verify(expression: x => x.GetAllApps(), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnAppWhenGetIgnoringFilters()
    {
        // Given
        App app = CreateRandomApp(id: 7);

        appBrokerMock.Setup(expression: x => x.GetAllAppsIgnoringFilters())
            .Returns(value: new[] { app }.AsQueryable());

        // When
        App result = appService.GetUnfilteredAppsAppOperation(appOperation: new AppOperation()).Apps.Single(predicate: app => app.Id == 7);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: app);

        appBrokerMock.Verify(expression: x => x.GetAllAppsIgnoringFilters(), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnNoVisibleAppWhenVisibleQueryIsEmpty()
    {
        // Given
        App app = CreateRandomApp(id: 9);

        appBrokerMock.Setup(expression: x => x.GetAllApps())
            .Returns(value: Array.Empty<cCoder.Data.Models.CMS.App>()
            .AsQueryable());

        // When
        App result = appService.GetVisibleAppsAppOperation(
            appOperation: new AppOperation()).Apps
            .SingleOrDefault(predicate: app => app.Id == 9);

        // Then
        result.Should()
            .BeNull();

        appBrokerMock.Verify(expression: x => x.GetAllApps(), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

}