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
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = TestUsers.WithPrivilege("commonobject_create");
        CommonObject commonObject = CreateRandomCommonObject(
            "Core/Other"
        );
        commonObject.Id = 99;
        commonObject.Version = 9;

        commonObjectCacheMock
            .SetupGet(x => x.LatestSet)
            .Returns(Array.Empty<DataCommonObject>());

        currentUser = actor;
        commonObjectServiceMock.Setup(x => x.AddAsync(commonObject)).ReturnsAsync(commonObject);

        // When
        cCoder.ContentManagement.Models.Result<CommonObject>[] results = (
            await commonObjectProcessingService.ImportAsync(new[] { commonObject })
        ).ToArray();

        // Then
        results.Should().ContainSingle();
        results[0].Success.Should().BeTrue();
        commonObject.Id.Should().Be(0);
        commonObject.Version.Should().Be(1);
        commonObjectServiceMock.Verify(x => x.AddAsync(commonObject), Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyGet(x => x.LatestSet, Times.Once);
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldPromoteVersionAndUpdateWhenItemIsNewerThanExistingForImport()
    {
        // When

        // Then
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = TestUsers.WithPrivileges(["commonobject_create", "commonobject_update"]);
        CommonObject dbObject = CreateRandomCommonObject("Core/Other");
        dbObject.Version = 4;
        dbObject.CreatedOn = DateTimeOffset.UtcNow.AddHours(-2);
        dbObject.LastUpdated = DateTimeOffset.UtcNow.AddHours(-2);

        CommonObject incoming = CreateRandomCommonObject("Core/Other");
        incoming.Name = dbObject.Name;
        incoming.Key = dbObject.Key;
        incoming.Culture = dbObject.Culture;
        incoming.Type = dbObject.Type;
        incoming.CreatedOn = DateTimeOffset.UtcNow;
        incoming.LastUpdated = DateTimeOffset.UtcNow;

        commonObjectCacheMock.SetupGet(x => x.LatestSet).Returns(new[] { new DataCommonObject
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
        commonObjectServiceMock.Setup(x => x.GetAll()).Returns(new[] { dbObject }.AsQueryable());
        commonObjectServiceMock.Setup(x => x.AddAsync(incoming)).ReturnsAsync(incoming);

        // When
        cCoder.ContentManagement.Models.Result<CommonObject>[] results = (
            await commonObjectProcessingService.ImportAsync(new[] { incoming })
        ).ToArray();

        // Then
        results.Should().ContainSingle();
        results[0].Success.Should().BeTrue();
        incoming.Id.Should().Be(0);
        incoming.Version.Should().Be(5);
        commonObjectServiceMock.Verify(x => x.GetAll(), Times.Exactly(2));
        commonObjectServiceMock.Verify(x => x.AddAsync(incoming), Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyGet(x => x.LatestSet, Times.Once);
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoResultsWhenItemMatchesExistingAndIsNotNewerForImport()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        CommonObject dbObject = CreateRandomCommonObject("Core/Other");
        CommonObject incoming = CreateRandomCommonObject("Core/Other");
        incoming.Name = dbObject.Name;
        incoming.Key = dbObject.Key;
        incoming.Culture = dbObject.Culture;
        incoming.Type = dbObject.Type;
        incoming.CreatedOn = dbObject.CreatedOn;
        incoming.LastUpdated = dbObject.LastUpdated;

        commonObjectCacheMock.SetupGet(x => x.LatestSet).Returns(new[] { new DataCommonObject
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
        cCoder.ContentManagement.Models.Result<CommonObject>[] results = (
            await commonObjectProcessingService.ImportAsync(new[] { incoming })
        ).ToArray();

        // Then
        results.Should().BeEmpty();
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyGet(x => x.LatestSet, Times.Once);
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}





















