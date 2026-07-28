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
using cCoder.ContentManagement.Models;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldResolveAuthorizationAndRenderComponent()
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

        string expectedHtml = "<section>component</section>";

        authorizationProcessingServiceMock
            .Setup(expression: service => service.ResolveRenderAuthorization(
                culture: null))
            .Returns(value: authorization);

        componentRenderProcessingServiceMock
            .Setup(
                expression: service =>
                    service.RenderComponentRenderOperation(
                        operation: It.Is<ComponentRenderOperation>(
                            match: operation =>
                                operation.AppId == 1
                                && operation.Name == "Hero"
                                && operation.User == user
                                && operation.Culture == "en-GB"
                                && operation.Theme == "Default")))
            .Returns(value: expectedHtml);

        // When
        string result = renderOrchestrationService.Render(
            appId: 1,
            name: "Hero",
            culture: null,
            theme: "Default");

        // Then
        result.Should()
            .Be(expected: expectedHtml);

        authorizationProcessingServiceMock.VerifyAll();
        componentRenderProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldRenderComponentThroughProcessingService()
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

        string expectedHtml = "<section>component</section>";

        componentRenderProcessingServiceMock
            .Setup(
                expression: service =>
                    service.RenderComponentRenderOperation(
                        operation: It.Is<ComponentRenderOperation>(
                            match: operation =>
                                operation.AppId == 1
                                && operation.Name == "Hero"
                                && operation.User == user
                                && operation.Culture == "en-GB"
                                && operation.Theme == "Default")))
            .Returns(value: expectedHtml);

        // When
        string result = renderOrchestrationService.RenderUser(appId: 1, name: "Hero", user: user, culture: "en-GB", theme: "Default");

        // Then
        result.Should()
            .Be(expected: expectedHtml);

        componentRenderProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenUserIsNull()
    {
        // Given
        // When
        Action act = () => renderOrchestrationService.RenderUser(appId: 1, name: "Hero", user: null!, culture: "en-GB", theme: "Default");

        // Then
        act.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "user is required.");
    }
}