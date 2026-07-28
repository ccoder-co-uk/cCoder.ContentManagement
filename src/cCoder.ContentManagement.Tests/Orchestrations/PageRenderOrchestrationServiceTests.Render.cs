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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.Core.Services.Tests;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public partial class PageRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldDelegatePreparedPageToRenderProcessingService()
    {
        // Given
        Page page = new()
        {
            Id = 10,
            AppId = 1,
            Name = "Home",
            Path = string.Empty,
            App = new App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
                DefaultCultureId = string.Empty,
                DefaultTheme = "Default",
                ConfigJson = "{}",
                Layouts = [],
                Pages = [],
                Components = [],
                Resources = [],
                Scripts = [],
                Templates = []
            },
            PageInfo = [],
            Contents = [],
            Roles = []
        };

        User user = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        RenderResult expected = new() { StatusCode = 200 };
        Mock<IPageRenderProcessingService> processingServiceMock = new();
        Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock = new();

        PageRenderOrchestrationService orchestrationService = new(
            pageRenderProcessingService: processingServiceMock.Object,
            authorizationProcessingService: authorizationProcessingServiceMock.Object);

        processingServiceMock
            .Setup(expression: service => service.RenderPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.SourcePage == page
                    && operation.User == user
                    && operation.Theme == "Default"
                    && operation.Culture == string.Empty
                    && operation.Edit)))
            .Returns(valueFunction: (PageRenderOperation operation) =>
            {
                operation.Page = expected;

                return operation;
            });

        // When
        RenderResult actual = orchestrationService.RenderPageUserRenderResult(page: page, user: user, theme: "Default", culture: string.Empty, edit: true);

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        processingServiceMock.VerifyAll();
    }
}