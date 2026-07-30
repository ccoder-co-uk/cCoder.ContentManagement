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

using FluentAssertions;
using Moq;
using Xunit;

using DataCommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldResetIdentityAndAddItWhenItemIsNewForImport()
    {
        // Given
        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationManagerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivilege(privilege: "commonobject_create");

        CommonObject commonObject = CreateRandomCommonObject(
type: "Core/Other"
        );

        commonObject.Id = 99;
        commonObject.Version = 9;

        commonObjectCacheMock
            .Setup(expression: x => x.GetLatestSet())
            .Returns(value: Array.Empty<DataCommonObject>());

        currentUser = actor;

        commonObjectServiceMock.Setup(expression: x => x.AddCommonObjectAsync(newCommonObject: commonObject))
            .ReturnsAsync(value: commonObject);

        // When
        OperationResult<CommonObject>[] results = (
            await commonObjectProcessingService.ImportCommonObjectResultAsync(items: new[] { commonObject })
        ).ToArray();

        // Then
        results.Should()
            .ContainSingle();

        results[0].Success.Should()
            .BeTrue();

        commonObject.Id.Should()
            .Be(expected: 0);

        commonObject.Version.Should()
            .Be(expected: 1);

        commonObjectServiceMock.Verify(expression: x => x.AddCommonObjectAsync(newCommonObject: commonObject), times: Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.Verify(expression: x => x.GetLatestSet(), times: Times.Once);
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldPromoteVersionAndUpdateWhenItemIsNewerThanExistingForImport()
    {

        // Given
        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationManagerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivileges(privileges: ["commonobject_create", "commonobject_update"]);
        CommonObject dbObject = CreateRandomCommonObject(type: "Core/Other");
        dbObject.Version = 4;
        dbObject.CreatedOn = DateTimeOffset.UtcNow.AddHours(hours: -2);
        dbObject.LastUpdated = DateTimeOffset.UtcNow.AddHours(hours: -2);

        CommonObject incoming = CreateRandomCommonObject(type: "Core/Other");
        incoming.Name = dbObject.Name;
        incoming.Key = dbObject.Key;
        incoming.Culture = dbObject.Culture;
        incoming.Type = dbObject.Type;
        incoming.CreatedOn = DateTimeOffset.UtcNow;
        incoming.LastUpdated = DateTimeOffset.UtcNow;

        commonObjectCacheMock.Setup(expression: x => x.GetLatestSet())
            .Returns(value: new[] { new DataCommonObject
        {
            Id = dbObject.Id,
            Name = dbObject.Name,
            Description = dbObject.Description,
            LastUpdated = dbObject.LastUpdated,
            LastUpdatedBy = dbObject.LastUpdatedBy,
            CreatedOn = dbObject.CreatedOn,
            CreatedBy = dbObject.CreatedBy,
            Version = dbObject.Version,
            Key = dbObject.Key,
            Type = dbObject.Type,
            Json = dbObject.Json,
            Culture = dbObject.Culture,
        } });

        currentUser = actor;

        commonObjectServiceMock.Setup(expression: x => x.GetAllCommonObject())
            .Returns(value: new[] { dbObject }.AsQueryable());

        commonObjectServiceMock.Setup(expression: x => x.AddCommonObjectAsync(newCommonObject: incoming))
            .ReturnsAsync(value: incoming);

        // When
        OperationResult<CommonObject>[] results = (
            await commonObjectProcessingService.ImportCommonObjectResultAsync(items: new[] { incoming })
        ).ToArray();

        // Then
        results.Should()
            .ContainSingle();

        results[0].Success.Should()
            .BeTrue();

        incoming.Id.Should()
            .Be(expected: 0);

        incoming.Version.Should()
            .Be(expected: 5);

        commonObjectServiceMock.Verify(expression: x => x.GetAllCommonObject(), times: Times.Exactly(callCount: 2));
        commonObjectServiceMock.Verify(expression: x => x.AddCommonObjectAsync(newCommonObject: incoming), times: Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.Verify(expression: x => x.GetLatestSet(), times: Times.Once);
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoResultsWhenItemMatchesExistingAndIsNotNewerForImport()
    {
        // Given
        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationManagerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        CommonObject dbObject = CreateRandomCommonObject(type: "Core/Other");
        CommonObject incoming = CreateRandomCommonObject(type: "Core/Other");
        incoming.Name = dbObject.Name;
        incoming.Key = dbObject.Key;
        incoming.Culture = dbObject.Culture;
        incoming.Type = dbObject.Type;
        incoming.CreatedOn = dbObject.CreatedOn;
        incoming.LastUpdated = dbObject.LastUpdated;

        commonObjectCacheMock.Setup(expression: x => x.GetLatestSet())
            .Returns(value: new[] { new DataCommonObject
        {
            Id = dbObject.Id,
            Name = dbObject.Name,
            Description = dbObject.Description,
            LastUpdated = dbObject.LastUpdated,
            LastUpdatedBy = dbObject.LastUpdatedBy,
            CreatedOn = dbObject.CreatedOn,
            CreatedBy = dbObject.CreatedBy,
            Version = dbObject.Version,
            Key = dbObject.Key,
            Type = dbObject.Type,
            Json = dbObject.Json,
            Culture = dbObject.Culture,
        } });

        // When
        OperationResult<CommonObject>[] results = (
            await commonObjectProcessingService.ImportCommonObjectResultAsync(items: new[] { incoming })
        ).ToArray();

        // Then
        results.Should()
            .BeEmpty();

        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.Verify(expression: x => x.GetLatestSet(), times: Times.Once);
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}