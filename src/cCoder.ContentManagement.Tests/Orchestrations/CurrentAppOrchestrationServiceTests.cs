// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed class CurrentAppOrchestrationServiceTests
{
    [Fact]
    public void ResolveCurrentApp_WhenWebDavPathContainsAppId_ReturnsVisibleApp()
    {
        // Given
        App app = new() { Id = 7 };
        Mock<IAppService> appService = new(MockBehavior.Strict);
        Mock<IHttpContextService> httpContextService = new(MockBehavior.Strict);

        httpContextService.Setup(expression: service => service.GetRequestPath())
            .Returns(value: "/api/webdav/Core/App(7)/DAV/file.txt");

        appService.Setup(expression: service =>
            service.GetVisibleAppAppOperation(It.Is<AppOperation>(operation => operation.AppId == 7)))
            .Returns(valueFunction: (AppOperation operation) =>
            {
                operation.App = app;
                return operation;
            });

        CurrentAppOrchestrationService service = new(
            appService: appService.Object,
            httpContextService: httpContextService.Object);

        // When
        App result = service.ResolveCurrentApp();

        // Then
        result.Should().BeSameAs(expected: app);
        appService.VerifyAll();
        httpContextService.VerifyAll();
    }

    [Fact]
    public void ResolveCurrentApp_WhenPathIsNotWebDav_ReturnsVisibleHostApp()
    {
        // Given
        App app = new() { Domain = "tenant.test" };
        Mock<IAppService> appService = new(MockBehavior.Strict);
        Mock<IHttpContextService> httpContextService = new(MockBehavior.Strict);

        httpContextService.Setup(expression: service => service.GetRequestPath())
            .Returns(value: "/api/dms/file.txt");

        httpContextService.Setup(expression: service => service.GetRequestHost())
            .Returns(value: "tenant.test");

        appService.Setup(expression: service =>
            service.GetVisibleAppsAppOperation(It.IsAny<AppOperation>()))
            .Returns(valueFunction: (AppOperation operation) =>
            {
                operation.Apps = new[] { app }.AsQueryable();
                return operation;
            });

        CurrentAppOrchestrationService service = new(
            appService: appService.Object,
            httpContextService: httpContextService.Object);

        // When
        App result = service.ResolveCurrentApp();

        // Then
        result.Should().BeSameAs(expected: app);
        appService.VerifyAll();
        httpContextService.VerifyAll();
    }
}