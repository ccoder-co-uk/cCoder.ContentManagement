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
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnRenderServiceResult()
    {
        // Given
        object model = new();

        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = "en-GB",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };

        templateRenderProcessingServiceMock
            .Setup(
                expression: service =>
                    service.RenderTemplateRenderOperation(
                        operation: It.Is<TemplateRenderOperation>(
                            match: operation =>
                                operation.AppId == 1
                                && operation.Name == "template"
                                && operation.Model == model
                                && operation.User == user
                                && operation.Culture == "en-GB")))
            .Returns(value: "rendered");

        // When
        string result = renderOrchestrationService.RenderUser(appId: 1, name: "template", culture: "en-GB", model: model, user: user);

        // Then
        result.Should()
            .Be(expected: "rendered");

        templateRenderProcessingServiceMock.VerifyAll();
    }

}