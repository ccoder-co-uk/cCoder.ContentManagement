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

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldRenderChildMenuItemsWhenMenuFor()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        Page child = CreateRandomPage();
        child.ParentId = 10;
        child.Path = "docs";
        child.ShowOnMenus = true;
        child.Order = 1;

        child.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Docs",
                PageId = child.Id,
                Page = null!,
                Culture = null!,
            },
        ];

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { child }.AsQueryable());

        // When
        string result = pageProcessingService.MenuFor(pageId: 10, culture: string.Empty);

        // Then
        result.Should()
            .Contain(expected: "<ul class='submenu'>");

        result.Should()
            .Contain(expected: "/docs");

        result.Should()
            .Contain(expected: "Docs");

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRenderEmptySubmenuWhenNoVisibleChildrenExistForMenuFor()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        // When
        string result = pageProcessingService.MenuFor(pageId: 10, culture: string.Empty);

        // Then
        result.Should()
            .Be(expected: "<ul class='submenu'></ul>");

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}