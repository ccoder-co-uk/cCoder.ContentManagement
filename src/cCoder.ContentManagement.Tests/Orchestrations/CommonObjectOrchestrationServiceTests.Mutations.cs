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
        CommonObject entity = CreateRandomCommonObject();
        CommonObject[] items = [entity];
        OperationResult<CommonObject>[] results = [new() { Success = true, Item = entity }];
        cacheProcessingServiceMock.Setup(service => service.GetLatestCommonObjects()).Returns([]);
        SetupAuthorization("commonobject_create");
        commonObjectProcessingServiceMock
            .Setup(service => service.AddAllCommonObjectsAsync(items, It.IsAny<IEnumerable<CommonObject>>(), CurrentUserId))
            .ReturnsAsync(results);
        cacheProcessingServiceMock.Setup(service => service.RefreshCommonObjects(1));

        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(items);

        actual.Should().BeSameAs(results);
        cacheProcessingServiceMock.Verify(service => service.RefreshCommonObjects(1), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRequestZeroChangedRefreshWhenImportChangesNoObjectsAsync()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        cacheProcessingServiceMock.Setup(service => service.GetLatestCommonObjects()).Returns([]);
        SetupAuthorization("commonobject_create");
        commonObjectProcessingServiceMock
            .Setup(service => service.AddAllCommonObjectsAsync(items, It.IsAny<IEnumerable<CommonObject>>(), CurrentUserId))
            .ReturnsAsync([]);
        cacheProcessingServiceMock.Setup(service => service.RefreshCommonObjects(0));

        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(items);

        actual.Should().BeEmpty();
        cacheProcessingServiceMock.Verify(service => service.RefreshCommonObjects(0), Times.Once);
    }

    [Fact]
    public async Task ShouldNotPersistOrRefreshWhenImportAuthorizationIsDeniedAsync()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        cacheProcessingServiceMock.Setup(service => service.GetLatestCommonObjects()).Returns([]);
        authorizationProcessingServiceMock
            .Setup(service => service.AuthorizeAuthorizationContext(
                It.IsAny<AuthorizationContext>()))
            .Throws(new SecurityException("Access Denied!"));

        Func<Task> action = async () =>
            await orchestrationService.AddAllCommonObjectsAsync(items);

        await action.Should().ThrowAsync<ContentManagementSecurityException>();
        commonObjectProcessingServiceMock.Verify(
            service => service.AddAllCommonObjectsAsync(
                It.IsAny<CommonObject[]>(),
                It.IsAny<IEnumerable<CommonObject>>(),
                It.IsAny<string>()),
            Times.Never);
        cacheProcessingServiceMock.Verify(
            service => service.RefreshCommonObjects(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ShouldAuthorizeCreateAndUpdatePersistThenRefreshWhenUpdateAsync()
    {
        CommonObject entity = CreateRandomCommonObject();
        SetupAuthorization("commonobject_create");
        SetupAuthorization("commonobject_update");
        commonObjectProcessingServiceMock
            .Setup(service => service.UpdateCommonObjectAsync(entity, CurrentUserId))
            .ReturnsAsync(entity);
        cacheProcessingServiceMock.Setup(service => service.RefreshCommonObjects(1));

        CommonObject actual = await orchestrationService.UpdateCommonObjectAsync(entity);

        actual.Should().BeSameAs(entity);
        cacheProcessingServiceMock.Verify(service => service.RefreshCommonObjects(1), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.Verify(service =>
            service.AuthorizeAuthorizationContext(It.IsAny<AuthorizationContext>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task ShouldNotRefreshWhenUpdatePersistenceFailsAsync()
    {
        CommonObject entity = CreateRandomCommonObject();
        SetupAuthorization("commonobject_create");
        SetupAuthorization("commonobject_update");
        commonObjectProcessingServiceMock
            .Setup(service => service.UpdateCommonObjectAsync(entity, CurrentUserId))
            .ThrowsAsync(new InvalidOperationException("persistence failed"));

        Func<Task> action = async () =>
            await orchestrationService.UpdateCommonObjectAsync(entity);

        await action.Should().ThrowAsync<ContentManagementDependencyException>();
        cacheProcessingServiceMock.Verify(
            service => service.RefreshCommonObjects(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ShouldAuthorizeDeletePersistThenRefreshWhenDeleteAsync()
    {
        SetupAuthorization("commonobject_delete");
        commonObjectProcessingServiceMock.Setup(service => service.DeleteAsync(42))
            .Returns(ValueTask.CompletedTask);
        cacheProcessingServiceMock.Setup(service => service.RefreshCommonObjects(1));

        await orchestrationService.DeleteAsync(42);

        cacheProcessingServiceMock.Verify(service => service.RefreshCommonObjects(1), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.Verify(service =>
            service.AuthorizeAuthorizationContext(It.IsAny<AuthorizationContext>()),
            Times.Once);
    }

    [Fact]
    public async Task ShouldAuthorizeDeleteAllPersistThenRefreshOnceAsync()
    {
        CommonObject[] items = [CreateRandomCommonObject(), CreateRandomCommonObject()];
        SetupAuthorization("commonobject_delete");
        commonObjectProcessingServiceMock.Setup(service => service.DeleteAllCommonObjectAsync(items))
            .Returns(ValueTask.CompletedTask);
        cacheProcessingServiceMock.Setup(service => service.RefreshCommonObjects(2));

        await orchestrationService.DeleteAllCommonObjectAsync(items);

        cacheProcessingServiceMock.Verify(service => service.RefreshCommonObjects(2), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        cacheProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.Verify(service =>
            service.AuthorizeAuthorizationContext(It.IsAny<AuthorizationContext>()),
            Times.Once);
    }

    [Fact]
    public void ShouldReadLatestObjectsFromCacheByType()
    {
        CommonObject included = CreateRandomCommonObject();
        included.Type = "Core/Resource";
        CommonObject excluded = CreateRandomCommonObject();
        excluded.Type = "Core/Script";
        cacheProcessingServiceMock.Setup(service => service.GetLatestCommonObjects())
            .Returns([included, excluded]);

        CommonObject[] actual = orchestrationService.LatestCommonObject(included.Type).ToArray();

        actual.Should().Equal(included);
        cacheProcessingServiceMock.VerifyAll();
    }
}