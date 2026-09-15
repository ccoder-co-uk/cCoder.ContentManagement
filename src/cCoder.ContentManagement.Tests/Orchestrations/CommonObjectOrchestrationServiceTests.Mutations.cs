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
        commonObjectProcessingServiceMock.Setup(service => service.GetLatestCommonObjects()).Returns([]);
        SetupAuthorization("commonobject_create");
        commonObjectProcessingServiceMock
            .Setup(service => service.AddAllCommonObjectsAsync(items, It.IsAny<IEnumerable<CommonObject>>(), CurrentUserId))
            .ReturnsAsync(results);
        commonObjectProcessingServiceMock.Setup(service => service.RefreshCommonObjects());

        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(items);

        actual.Should().BeSameAs(results);
        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        commonObjectProcessingServiceMock.VerifyAll();
        authorizationProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotRefreshWhenImportChangesNoObjectsAsync()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        commonObjectProcessingServiceMock.Setup(service => service.GetLatestCommonObjects()).Returns([]);
        SetupAuthorization("commonobject_create");
        commonObjectProcessingServiceMock
            .Setup(service => service.AddAllCommonObjectsAsync(items, It.IsAny<IEnumerable<CommonObject>>(), CurrentUserId))
            .ReturnsAsync([]);

        IEnumerable<OperationResult<CommonObject>> actual =
            await orchestrationService.AddAllCommonObjectsAsync(items);

        actual.Should().BeEmpty();
        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Never);
    }

    [Fact]
    public async Task ShouldNotPersistOrRefreshWhenImportAuthorizationIsDeniedAsync()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        commonObjectProcessingServiceMock.Setup(service => service.GetLatestCommonObjects()).Returns([]);
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
        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Never);
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
        commonObjectProcessingServiceMock.Setup(service => service.RefreshCommonObjects());

        CommonObject actual = await orchestrationService.UpdateCommonObjectAsync(entity);

        actual.Should().BeSameAs(entity);
        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        commonObjectProcessingServiceMock.VerifyAll();
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
        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Never);
    }

    [Fact]
    public async Task ShouldAuthorizeDeletePersistThenRefreshWhenDeleteAsync()
    {
        SetupAuthorization("commonobject_delete");
        commonObjectProcessingServiceMock.Setup(service => service.DeleteAsync(42))
            .Returns(ValueTask.CompletedTask);
        commonObjectProcessingServiceMock.Setup(service => service.RefreshCommonObjects());

        await orchestrationService.DeleteAsync(42);

        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        commonObjectProcessingServiceMock.VerifyAll();
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
        commonObjectProcessingServiceMock.Setup(service => service.RefreshCommonObjects());

        await orchestrationService.DeleteAllCommonObjectAsync(items);

        commonObjectProcessingServiceMock.Verify(service => service.RefreshCommonObjects(), Times.Once);
        commonObjectProcessingServiceMock.VerifyAll();
        commonObjectProcessingServiceMock.VerifyAll();
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
        commonObjectProcessingServiceMock.Setup(service => service.GetLatestCommonObjects())
            .Returns([included, excluded]);

        CommonObject[] actual = orchestrationService.LatestCommonObject(included.Type).ToArray();

        actual.Should().Equal(included);
        commonObjectProcessingServiceMock.VerifyAll();
    }
}