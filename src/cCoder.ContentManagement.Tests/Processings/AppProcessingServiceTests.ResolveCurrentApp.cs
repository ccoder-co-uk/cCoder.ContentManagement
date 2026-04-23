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
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = CreateRandomApp();
        app.Id = 7;
        DefaultHttpContext context = new();
        context.Request.Path = "/api/webdav/Core/App(7)/DAV/folder/file.txt";

        AppProcessingService serviceWithContext = new(
            appServiceMock.Object,
            cultureServiceMock.Object,
            privilegeBrokerMock.Object,
            appEventProcessingServiceMock.Object,
            authorizationBrokerMock.Object,
            roleBrokerMock.Object,
            userRoleBrokerMock.Object,
            context
        );

        appServiceMock.Setup(x => x.Get(7)).Returns(app);

        // When
        App result = serviceWithContext.ResolveCurrentApp();

        // Then
        result.Should().BeSameAs(app);
        appServiceMock.Verify(x => x.Get(7), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnAppByHostWhenRequestIsNotWebDavForResolveCurrentApp()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = CreateRandomApp();
        app.Domain = "tenant.test";

        DefaultHttpContext context = new();
        context.Request.Path = "/api/dms/folder/file.txt";
        context.Request.Host = new HostString("tenant.test");

        AppProcessingService serviceWithContext = new(
            appServiceMock.Object,
            cultureServiceMock.Object,
            privilegeBrokerMock.Object,
            appEventProcessingServiceMock.Object,
            authorizationBrokerMock.Object,
            roleBrokerMock.Object,
            userRoleBrokerMock.Object,
            context
        );

        appServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());

        // When
        App result = serviceWithContext.ResolveCurrentApp();

        // Then
        result.Should().BeSameAs(app);
        appServiceMock.Verify(x => x.GetAll(false), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}

















