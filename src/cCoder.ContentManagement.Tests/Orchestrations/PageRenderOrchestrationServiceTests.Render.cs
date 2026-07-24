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
        PageRenderOrchestrationService orchestrationService = new(config: new Config(), pageRenderProcessingService: processingServiceMock.Object);

        processingServiceMock
            .Setup(expression: x => x.RenderPageUserConfigRenderResult(page: page, user: user, config: It.IsAny<Config>(), theme: "Default", culture: string.Empty, edit: true))
            .Returns(value: expected);

        // When
        RenderResult actual = orchestrationService.RenderPageUserRenderResult(page: page, user: user, theme: "Default", culture: string.Empty, edit: true);

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        processingServiceMock.VerifyAll();
    }
}