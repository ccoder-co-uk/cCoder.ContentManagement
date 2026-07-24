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
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateRenderOrchestrationServiceTests
{
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
            .Setup(expression: x => x.RenderUserConfig(
appId: 1,
name: "Welcome",
model: model,
user: user,
culture: "en-GB",
config: It.IsAny<Config>(),
log: It.IsAny<ILogger>()))
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