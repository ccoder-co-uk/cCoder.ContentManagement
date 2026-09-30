// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task ShouldAuthorizePersistAndRaiseImportEventWhenImportChangesObjectsAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();
        CommonObject[] items = [entity];
        OperationResult<CommonObject>[] results = [new() { Success = true, Item = entity }];

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: []);

        ShouldSetupAuthorization(privilege: "commonobject_create");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: items,
                latestCommonObjects: It.IsAny<IEnumerable<CommonObject>>(),
                userId: CurrentUserId))
            .ReturnsAsync(value: results);

        eventProcessingServiceMock
            .Setup(expression: service => service.RaiseCommonObjectsImportedEventAsync(
                commonObjects: It.Is<CommonObject[]>(match: values =>
                    values.Length == 1 && values[0] == entity),
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);


        // When
        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        actual.Should()
            .BeSameAs(expected: results);

        commonObjectProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.VerifyAll();
        eventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotRaiseImportEventWhenImportChangesNoObjectsAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject()];

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: []);

        ShouldSetupAuthorization(privilege: "commonobject_create");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: items,
                latestCommonObjects: It.IsAny<IEnumerable<CommonObject>>(),
                userId: CurrentUserId))
            .ReturnsAsync(value: []);

        // When
        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        actual.Should()
            .BeEmpty();

        eventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotPersistOrRaiseEventWhenImportAuthorizationIsDeniedAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject()];

        commonObjectProcessingServiceMock
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

        eventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAuthorizePersistAndRaiseUpdateEventWhenUpdateAsync()
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

        eventProcessingServiceMock
            .Setup(expression: service => service.RaiseCommonObjectUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);


        // When
        CommonObject actual = await orchestrationService.UpdateCommonObjectAsync(
            updatedCommonObject: entity);

        // Then
        actual.Should()
            .BeSameAs(expected: entity);

        commonObjectProcessingServiceMock.VerifyAll();

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()),
            times: Times.Exactly(callCount: 2));

        eventProcessingServiceMock.VerifyAll();

    }

    [Fact]
    public async Task ShouldNotRaiseEventWhenUpdatePersistenceFailsAsync()
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

        eventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAuthorizePersistAndRaiseDeleteEventWhenDeleteAsync()
    {
        // Given
        ShouldSetupAuthorization(privilege: "commonobject_delete");

        CommonObject entity = CreateRandomCommonObject();
        entity.Id = 42;

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.GetCommonObject(commonObjectId: 42))
            .Returns(value: entity);

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.DeleteAsync(
                commonObjectId: 42))
            .Returns(value: ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(expression: service => service.RaiseCommonObjectDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(commonObjectId: 42);

        // Then
        commonObjectProcessingServiceMock.VerifyAll();

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()),
            times: Times.Once);

        eventProcessingServiceMock.VerifyAll();

    }

    [Fact]
    public async Task ShouldAuthorizePersistAndRaiseDeleteEventsWhenDeleteAllAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject(), CreateRandomCommonObject()];
        ShouldSetupAuthorization(privilege: "commonobject_delete");

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.DeleteAllCommonObjectAsync(
                deletedCommonObject: items))
            .Returns(value: ValueTask.CompletedTask);

        foreach (CommonObject item in items)
        {
            eventProcessingServiceMock
                .Setup(expression: service => service.RaiseCommonObjectDeleteEventAsync(
                    entity: item,
                    userId: CurrentUserId))
                .Returns(value: ValueTask.CompletedTask);
        }

        // When
        await orchestrationService.DeleteAllCommonObjectAsync(deletedCommonObject: items);

        // Then
        commonObjectProcessingServiceMock.VerifyAll();

        authorizationProcessingServiceMock.Verify(
            expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()),
            times: Times.Once);

        eventProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldReadLatestObjectsFromDataCacheByType()
    {
        // Given
        CommonObject included = CreateRandomCommonObject();
        included.Type = "Core/Resource";
        CommonObject excluded = CreateRandomCommonObject();
        excluded.Type = "Core/Script";

        commonObjectProcessingServiceMock
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: [included, excluded]);

        // When
        CommonObject[] actual = orchestrationService
            .LatestCommonObjects(type: included.Type)
            .ToArray();

        // Then
        actual.Should()
            .Equal(elements: included);

        commonObjectProcessingServiceMock.VerifyAll();
    }
}