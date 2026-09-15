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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldUpdateAppWhenStoredAppExistsForUpdateAsync()
    {
        // Given
        App dbApp = CreateRandomApp();
        dbApp.Id = 1;
        dbApp.Cultures = null!;
        App app = CreateRandomApp();
        app.Id = dbApp.Id;
        app.Cultures = null!;

        appServiceMock.Setup(expression: x => x.GetApp(appId: dbApp.Id, ignoreFilters: true))
            .Returns(value: dbApp);

        appServiceMock.Setup(expression: x => x.UpdateAppAsync(updatedApp: dbApp))
            .ReturnsAsync(value: dbApp);

        // When
        App result = await appProcessingService.UpdateAppAsync(updatedApp: app);

        // Then

        result.Should()
            .BeSameAs(expected: dbApp);

        appServiceMock.Verify(expression: x => x.GetApp(appId: dbApp.Id, ignoreFilters: true), times: Times.Once);
        appServiceMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: dbApp), times: Times.Once);
        VerifyNoOtherAppServiceCalls();
    }

    [Fact]
    public async Task ShouldUpdateOnlyAppRowAndLeavePostedChildrenForEventHandlersAsync()
    {
        // Given
        App dbApp = CreateRandomApp();
        dbApp.Id = 1;
        dbApp.Cultures = null!;
        dbApp.Pages = null!;
        dbApp.Roles = null!;
        App postedApp = CreateRandomApp();
        postedApp.Id = dbApp.Id;

        appServiceMock
            .Setup(expression: service => service.GetApp(appId: dbApp.Id, ignoreFilters: true))
            .Returns(value: dbApp);

        appServiceMock
            .Setup(expression: service => service.UpdateAppAsync(updatedApp: dbApp))
            .ReturnsAsync(value: dbApp);

        // When
        App result = await appProcessingService
            .UpdateAppAsync(updatedApp: postedApp);

        // Then
        result.Should()
            .BeSameAs(expected: dbApp);

        dbApp.Cultures.Should()
            .BeNull();

        dbApp.Pages.Should()
            .BeNull();

        dbApp.Roles.Should()
            .BeNull();

        appServiceMock.Verify(
            expression: service => service.GetApp(
                appId: dbApp.Id,
                ignoreFilters: true),
            times: Times.Once);

        appServiceMock.Verify(
            expression: service => service.UpdateAppAsync(updatedApp: dbApp),
            times: Times.Once);

        VerifyNoOtherAppServiceCalls();
    }
}