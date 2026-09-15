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
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAuthorizeThenCallProcessingWhenAddAsync()
    {
        // Given
        App entity = CreateRandomApp();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == null
                    && context.Request.Privilege == "app_create")));

        appProcessingServiceMock
            .Setup(expression: x => x.AddAppAsync(newApp: entity))
            .ReturnsAsync(valueFunction: (App app) => app);

        // When
        App result = await orchestrationService.AddAppAsync(newApp: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        appProcessingServiceMock.Verify(
            expression: x => x.AddAppAsync(newApp: It.IsAny<App>()),
            times: Times.Once);

        appProcessingServiceMock.VerifyNoOtherCalls();
    }
}