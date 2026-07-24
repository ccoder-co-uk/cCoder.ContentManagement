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
using cCoder.ContentManagement.Models;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldResolveAuthorizationAndRenderTemplate()
    {
        // Given
        User user = new()
        {
            DefaultCultureId = "en-GB"
        };

        RenderAuthorization authorization = new()
        {
            Culture = "en-GB",
            User = user
        };

        object model = new { Name = "Ward" };
        string expectedHtml = "<main>template</main>";

        authorizationProcessingServiceMock
            .Setup(expression: service => service.ResolveRenderAuthorization(
                culture: null))
            .Returns(value: authorization);

        templateRenderProcessingServiceMock
            .Setup(
                expression: service =>
                    service.RenderTemplateRenderOperation(
                        operation: It.Is<TemplateRenderOperation>(
                            match: operation =>
                                operation.AppId == 1
                                && operation.Name == "Welcome"
                                && operation.Model == model
                                && operation.User == user
                                && operation.Culture == "en-GB")))
            .Returns(value: expectedHtml);

        // When
        string result = renderOrchestrationService.Render(
            appId: 1,
            name: "Welcome",
            culture: null,
            model: model);

        // Then
        result.Should()
            .Be(expected: expectedHtml);

        authorizationProcessingServiceMock.VerifyAll();
        templateRenderProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldRenderTemplateThroughProcessingService()
    {
        // Given
        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = "en-GB",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = []
        };

        object model = new { Name = "Ward" };
        string expectedHtml = "<main>template</main>";

        templateRenderProcessingServiceMock
            .Setup(
                expression: service =>
                    service.RenderTemplateRenderOperation(
                        operation: It.Is<TemplateRenderOperation>(
                            match: operation =>
                                operation.AppId == 1
                                && operation.Name == "Welcome"
                                && operation.Model == model
                                && operation.User == user
                                && operation.Culture == "en-GB")))
            .Returns(value: expectedHtml);

        // When
        string result = renderOrchestrationService.RenderUser(appId: 1, name: "Welcome", culture: "en-GB", model: model, user: user);

        // Then
        result.Should()
            .Be(expected: expectedHtml);

        templateRenderProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenUserIsNull()
    {
        // Given
        // When
        Action act = () => renderOrchestrationService.RenderUser(appId: 1, name: "Welcome", culture: "en-GB", model: new { }, user: null!);

        // Then
        act.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "user is required.");
    }
}