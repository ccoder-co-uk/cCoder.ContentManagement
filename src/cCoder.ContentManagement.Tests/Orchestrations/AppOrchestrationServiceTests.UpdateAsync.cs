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
    public async Task ShouldAuthorizeThenCallProcessingWhenUpdateAsync()
    {
        // Given
        App entity = CreateRandomApp();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == entity.Id
                    && context.Request.Privilege == "app_update")));

        appProcessingServiceMock.Setup(expression: x => x.UpdateAppAsync(updatedApp: entity))
            .ReturnsAsync(value: entity);

        // When
        App result = await orchestrationService.UpdateAppAsync(updatedApp: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        appProcessingServiceMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: entity), times: Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRaiseUpdateEventWithPostedGraphWhenStoredAppIsFlatAsync()
    {
        // Given
        App postedApp = CreateRandomApp();
        App storedApp = new() { Id = postedApp.Id, Name = postedApp.Name, Domain = postedApp.Domain };

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == postedApp.Id
                    && context.Request.Privilege == "app_update")));

        appProcessingServiceMock
            .Setup(expression: service => service.UpdateAppAsync(updatedApp: postedApp))
            .ReturnsAsync(value: storedApp);

        // When
        App result = await orchestrationService
            .UpdateAppAsync(updatedApp: postedApp);

        // Then
        result.Should()
            .BeSameAs(expected: storedApp);

        appProcessingServiceMock.Verify(
            expression: service => service.UpdateAppAsync(updatedApp: postedApp),
            times: Times.Once);

        appProcessingServiceMock.VerifyNoOtherCalls();
    }
}