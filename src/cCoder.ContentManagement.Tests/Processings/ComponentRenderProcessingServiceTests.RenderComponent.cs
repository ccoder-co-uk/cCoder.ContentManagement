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
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderComponent = cCoder.Data.Models.CMS.Component;
using RenderComponentParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderUser = cCoder.Data.Models.Security.User;
using cCoder.ContentManagement.Tests.Processings;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentRenderProcessingServiceTests
{
    [Fact]
    public async Task ShouldRenderDeclaredSupportedTagTypesForComponentRoot()
    {
        // Given
        (RenderApp app, RenderUser user, RenderComponent component, RenderComponentParams renderParams) =
            CreateComponentRenderContext();

        metadataCacheMock.Setup(expression: x => x.Get(key: "site-description", culture: "en-GB"))
            .Returns(value: "Meta Description");

        commonObjectCacheMock
            .Setup(expression: x => x.Get<RenderScript>(key: "script|bootstrap"))
            .Returns(value: new RenderScript { Name = "Bootstrap", Content = "cached-bootstrap" });

        renderFileContentServiceMock.Setup(expression: x => x.GetLatestTextContent(appId: app.Id, path: "snippets/info"))
            .Returns(value: "snippet-text");

        // When
        string result = await RenderTestWorkflowServer.RunStringAsync(
action: workflowBaseUrl => CreateSut(workflowBaseUrl: workflowBaseUrl)
            .RenderComponentComponentRenderParams(component: component, renderParams: renderParams));

        // Then
        result.Should()
            .Contain(expected: "snippet-text");

        result.Should()
            .Contain(expected: "bootstrap-script");

        result.Should()
            .Contain(expected: "Child Component");

        result.Should()
            .Contain(expected: "<script type='text/javascript'></script>");

        result.Should()
            .NotContain(unexpected: "defer async");

        result.Should()
            .Contain(expected: "Meta Description");

        result.Should()
            .Contain(expected: "Hello");

        result.Should()
            .Contain(expected: "Blue");

        result.Should()
            .Contain(expected: "executed");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenComponentIsNull()
    {
        // Given
        (_, _, _, RenderComponentParams renderParams) = CreateComponentRenderContext();

        // When
        Action act = () => CreateSut(workflowBaseUrl: "http://127.0.0.1/")
            .RenderComponentComponentRenderParams(component: null!, renderParams: renderParams);

        // Then
        act.Should()
            .Throw<ValidationException>();
    }
}