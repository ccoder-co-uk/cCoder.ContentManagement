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

using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentRenderOrchestrationServiceTests
{
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
            .Setup(expression: x => x.RenderUser(appId: 1, name: "Hero", user: user, culture: "en-GB", theme: "Default"))
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