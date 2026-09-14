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
using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Package entity = CreateRandomPackage();

        packageProcessingServiceMock.Setup(expression: x => x.AddPackageAsync(newPackage: entity))
            .ReturnsAsync(value: entity);

        packageEventProcessingServiceMock
            .Setup(expression: x => x.RaisePackageAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Package result = await orchestrationService.AddPackageAsync(newPackage: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        packageProcessingServiceMock.Verify(expression: x => x.AddPackageAsync(newPackage: entity), times: Times.Once);
        packageEventProcessingServiceMock.Verify(expression: x => x.RaisePackageAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

    [Fact]
    public async Task Package_WhenUserLacksCreatePrivilege_IsNotPersisted()
    {
        // Given
        Package entity = CreateRandomPackage();

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == null
                    && context.Request.Privilege == "Package_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddPackageAsync(newPackage: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        packageProcessingServiceMock.VerifyNoOtherCalls();
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
