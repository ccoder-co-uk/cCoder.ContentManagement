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
using System.Security;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldWalkToTopParentWhenGetRoot()
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

        Page root = CreateRandomPage();
        root.Id = 1;
        Page child = CreateRandomPage();
        child.Id = 2;
        child.ParentId = root.Id;

        pageServiceMock.Setup(expression: x => x.GetPage(pageId: root.Id))
            .Returns(value: root);

        pageServiceMock.Setup(expression: x => x.GetPage(pageId: child.Id))
            .Returns(value: child);

        // When
        Page result = pageProcessingService.GetRootPage(pageId: child.Id);

        // Then
        result.Id.Should()
            .Be(expected: root.Id);

        pageServiceMock.Verify(expression: x => x.GetPage(pageId: child.Id), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.GetPage(pageId: root.Id), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnSamePageWhenPageIsAlreadyRootForGetRoot()
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

        Page root = CreateRandomPage();
        root.Id = 1;
        root.ParentId = null;

        pageServiceMock.Setup(expression: x => x.GetPage(pageId: root.Id))
            .Returns(value: root);

        // When
        Page result = pageProcessingService.GetRootPage(pageId: root.Id);

        // Then
        result.Should()
            .BeSameAs(expected: root);

        pageServiceMock.Verify(expression: x => x.GetPage(pageId: root.Id), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}