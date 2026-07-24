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
using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Xunit;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderTemplate = cCoder.Data.Models.CMS.Template;
using RenderTemplateParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using RenderUser = cCoder.Data.Models.Security.User;
using cCoder.ContentManagement.Tests.Processings;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateRenderProcessingServiceTests
{
    [Fact]
    public async Task ShouldRenderDeclaredSupportedTagTypesForTemplateRoot()
    {
        TemplateRenderProcessingService sut = CreateSut();
        (RenderApp app, RenderUser user, RenderTemplate template) = CreateTemplateRenderContext();
        RenderTemplateParams renderParams = new(app: app, user: user, culture: "en-GB");

        metadataCacheMock.Setup(expression: x => x.Get(key: "site-description", culture: "en-GB"))
            .Returns(value: "Meta Description");

        commonObjectCacheMock
            .Setup(expression: x => x.Get<RenderScript>(key: "script|bootstrap"))
            .Returns(value: new RenderScript { Name = "Bootstrap", Content = "cached-bootstrap" });

        string result = await RenderTestWorkflowServer.RunAsync(action: workflowBaseUrl =>
            sut.RenderTemplateRenderParamsConfig(template: template, model: new { Name = "Taylor" }, renderParams: renderParams, config: CreateConfig(workflowBaseUrl: workflowBaseUrl)));

        result.Should()
            .Contain(expected: "App|Blue|Taylor|bootstrap-script|");

        result.Should()
            .Contain(expected: "Hero Taylor");

        result.Should()
            .Contain(expected: "<script type='text/javascript'></script>");

        result.Should()
            .NotContain(unexpected: "defer async");

        result.Should()
            .Contain(expected: "Meta Description");

        result.Should()
            .Contain(expected: "Hello");

        result.Should()
            .Contain(expected: "executed");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenTemplateIsNull()
    {
        TemplateRenderProcessingService sut = CreateSut();
        (RenderApp app, RenderUser user, _) = CreateTemplateRenderContext();

        Action act = () => sut.RenderTemplateRenderParamsConfig(template: null!, model: new { Name = "Taylor" }, renderParams: new RenderTemplateParams(app: app, user: user, culture: "en-GB"), config: CreateConfig(workflowBaseUrl: "http://127.0.0.1/"));

        act.Should()
            .Throw<ValidationException>();
    }
}