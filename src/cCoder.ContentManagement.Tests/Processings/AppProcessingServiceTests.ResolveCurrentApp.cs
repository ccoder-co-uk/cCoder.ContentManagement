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



using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public void ShouldReturnAppFromFoundationServiceGetWhenWebDavPathContainsAppIdForResolveCurrentApp()
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

        App app = CreateRandomApp();
        app.Id = 7;
        DefaultHttpContext context = new();
        context.Request.Path = "/api/webdav/Core/App(7)/DAV/folder/file.txt";

        AppProcessingService serviceWithContext = new(
service: appServiceMock.Object,
cultureBroker: cultureBrokerMock.Object,
privilegeBroker: privilegeBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object,
roleBroker: roleBrokerMock.Object,
userRoleBroker: userRoleBrokerMock.Object,
pageBroker: pageBrokerMock.Object,
httpContext: context
        );

        appServiceMock.Setup(expression: x => x.GetApp(appId: 7))
            .Returns(value: app);

        // When
        App result = serviceWithContext.ResolveCurrentApp();

        // Then

        result.Should()
            .BeSameAs(expected: app);

        appServiceMock.Verify(expression: x => x.GetApp(appId: 7), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnAppByHostWhenRequestIsNotWebDavForResolveCurrentApp()
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

        App app = CreateRandomApp();
        app.Domain = "tenant.test";

        DefaultHttpContext context = new();
        context.Request.Path = "/api/dms/folder/file.txt";
        context.Request.Host = new HostString(value: "tenant.test");

        AppProcessingService serviceWithContext = new(
service: appServiceMock.Object,
cultureBroker: cultureBrokerMock.Object,
privilegeBroker: privilegeBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object,
roleBroker: roleBrokerMock.Object,
userRoleBroker: userRoleBrokerMock.Object,
pageBroker: pageBrokerMock.Object,
httpContext: context
        );

        appServiceMock.Setup(expression: x => x.GetAllApp(ignoreFilters: false))
            .Returns(value: new[] { app }.AsQueryable());

        // When
        App result = serviceWithContext.ResolveCurrentApp();

        // Then

        result.Should()
            .BeSameAs(expected: app);

        appServiceMock.Verify(expression: x => x.GetAllApp(ignoreFilters: false), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}