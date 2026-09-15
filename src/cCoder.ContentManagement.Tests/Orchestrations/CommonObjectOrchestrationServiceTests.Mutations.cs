// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using System.Security;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAuthorizePersistAndRefreshOnceWhenImportChangesObjectsAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();
        CommonObject[] items = [entity];
        OperationResult<CommonObject>[] results = [new() { Success = true, Item = entity }];

        cacheProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: []);

        ShouldSetupAuthorization(privilege: "commonobject_create");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: items,
                latestCommonObjects: It.IsAny<IEnumerable<CommonObject>>(),
                userId: CurrentUserId))
            .ReturnsAsync(value: results);

        cacheProcessingServiceMock.Setup(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 1));

        // When
        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        actual.Should()
            .BeSameAs(expected: results);

        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 1),
            times: Times.Once);

        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRequestZeroChangedRefreshWhenImportChangesNoObjectsAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject()];

        cacheProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: []);

        ShouldSetupAuthorization(privilege: "commonobject_create");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: items,
                latestCommonObjects: It.IsAny<IEnumerable<CommonObject>>(),
                userId: CurrentUserId))
            .ReturnsAsync(value: []);

        cacheProcessingServiceMock.Setup(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 0));

        // When
        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        actual.Should()
            .BeEmpty();

        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 0),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldNotPersistOrRefreshWhenImportAuthorizationIsDeniedAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject()];

        cacheProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: []);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        commonObjectProcessingServiceMock.Verify(
            expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: It.IsAny<CommonObject[]>(),
                latestCommonObjects: It.IsAny<IEnumerable<CommonObject>>(),
                userId: It.IsAny<string>()),
            times: Times.Never);

        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(
                changedCommonObjectCount: It.IsAny<int>()),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldAuthorizeCreateAndUpdatePersistThenRefreshWhenUpdateAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();
        ShouldSetupAuthorization(privilege: "commonobject_create");
        ShouldSetupAuthorization(privilege: "commonobject_update");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.UpdateCommonObjectAsync(
                updatedCommonObject: entity,
                userId: CurrentUserId))
            .ReturnsAsync(value: entity);

        cacheProcessingServiceMock.Setup(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 1));

        // When
        CommonObject actual = await orchestrationService.UpdateCommonObjectAsync(
            updatedCommonObject: entity);

        // Then
        actual.Should()
            .BeSameAs(expected: entity);

        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 1),
            times: Times.Once);

        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task ShouldNotRefreshWhenUpdatePersistenceFailsAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();
        ShouldSetupAuthorization(privilege: "commonobject_create");
        ShouldSetupAuthorization(privilege: "commonobject_update");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.UpdateCommonObjectAsync(
                updatedCommonObject: entity,
                userId: CurrentUserId))
            .ThrowsAsync(exception: new InvalidOperationException(message: "persistence failed"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateCommonObjectAsync(updatedCommonObject: entity);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementDependencyException>();

        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(
                changedCommonObjectCount: It.IsAny<int>()),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldAuthorizeDeletePersistThenRefreshWhenDeleteAsync()
    {
        // Given
        ShouldSetupAuthorization(privilege: "commonobject_delete");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.DeleteAsync(commonObjectId: 42))
            .Returns(value: ValueTask.CompletedTask);

        cacheProcessingServiceMock.Setup(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 1));

        // When
        await orchestrationService.DeleteAsync(commonObjectId: 42);

        // Then
        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 1),
            times: Times.Once);

        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldAuthorizeDeleteAllPersistThenRefreshOnceAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject(), CreateRandomCommonObject()];
        ShouldSetupAuthorization(privilege: "commonobject_delete");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.DeleteAllCommonObjectAsync(
                deletedCommonObject: items))
            .Returns(value: ValueTask.CompletedTask);

        cacheProcessingServiceMock.Setup(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 2));

        // When
        await orchestrationService.DeleteAllCommonObjectAsync(deletedCommonObject: items);

        // Then
        cacheProcessingServiceMock.Verify(
            expression: service => service.RefreshCommonObjects(changedCommonObjectCount: 2),
            times: Times.Once);

        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()),
            times: Times.Once);
    }

    [Fact]
    public void ShouldReadLatestObjectsFromCacheByType()
    {
        // Given
        CommonObject included = CreateRandomCommonObject();
        included.Type = "Core/Resource";
        CommonObject excluded = CreateRandomCommonObject();
        excluded.Type = "Core/Script";

        cacheProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: [included, excluded]);

        // When
        CommonObject[] actual = orchestrationService
            .LatestCommonObject(type: included.Type)
            .ToArray();

        // Then
        actual.Should()
            .Equal(elements: included);

        cacheProcessingServiceMock.VerifyAll();
    }
}