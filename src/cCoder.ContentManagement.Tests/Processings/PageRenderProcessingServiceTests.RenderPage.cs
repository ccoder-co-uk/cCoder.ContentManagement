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
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderPage = cCoder.Data.Models.CMS.Page;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderUser = cCoder.Data.Models.Security.User;
using cCoder.ContentManagement.Tests.Processings;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRenderProcessingServiceTests
{
    [Fact]
    public async Task ShouldRenderDeclaredSupportedTagTypesForPageRoot()
    {
        // Given
        RenderApp app = CreateApp();
        RenderPage page = app.Pages.First(predicate: foundPage => foundPage.Id == 10);
        RenderUser user = CreateUser();

        metadataReaderBroker.Set(name: "site-description", culture: "en-GB", value: "Meta Description");

        // When
        PageRenderResult result = await RenderTestWorkflowServer.RunRenderResultAsync(action: workflowBaseUrl =>
        {
            PageRenderProcessingService sut = CreateSut(
                config: CreateConfig(workflowBaseUrl: workflowBaseUrl));

            return sut.RenderPageUserRenderResult(
                page: page,
                user: user,
                theme: "Default",
                culture: "en-GB");
        });

        // Then
        result.HeaderHtml.Should()
            .Contain(expected: "<title>Home</title>");

        result.HeaderHtml.Should()
            .Contain(expected: "Meta Description");

        result.HeaderHtml.Should()
            .Contain(expected: "bootstrap-script");

        result.HeaderHtml.Should()
            .Contain(expected: ".common { display: block; }");

        result.BodyHtml.Should()
            .Contain(expected: "Body Content");

        result.BodyHtml.Should()
            .Contain(expected: "Hero Component");

        result.BodyHtml.Should()
            .Contain(expected: "<script type='text/javascript' nonce='[request[nonce]]'>hero-component-script</script>");

        result.BodyHtml.Should()
            .Contain(expected: "Hello|Hi|Greeting Description");

        result.BodyHtml.Should()
            .Contain(expected: "Blue|App|");

        result.BodyHtml.Should()
            .Contain(expected: "executed");

        result.BodyHtml.Should()
            .Contain(expected: "href='/Summary'");

        result.BodyHtml.Should()
            .Contain(expected: "dropdown-menu");

        metadataReaderBroker.Requests.Should()
            .ContainSingle(predicate: request =>
            request.Name == "site-description" && request.Culture == "en-GB");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenPageIsNull()
    {
        // Given
        Config config = CreateConfig(
            workflowBaseUrl: "http://127.0.0.1/");

        PageRenderProcessingService sut = CreateSut(config: config);

        // When
        Action act = () => sut.RenderPageUserRenderResult(
            page: null!,
            user: CreateUser(),
            theme: "Default",
            culture: "en-GB");

        // Then
        act.Should()
            .Throw<ValidationException>();
    }
}