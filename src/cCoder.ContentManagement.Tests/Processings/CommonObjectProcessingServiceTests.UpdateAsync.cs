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



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldCreateNewVersionAndAddItWhenUserHasPrivilegesForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivileges(
privileges: new[] { "commonobject_create", "commonobject_update" }
        );

        CommonObject commonObject = CreateRandomCommonObject(
type: "Core/Other"
        );

        CommonObject existingVersion = CreateRandomCommonObject(
type: "Core/Other"
        );

        existingVersion.Name = commonObject.Name;
        existingVersion.Type = commonObject.Type;
        existingVersion.Culture = commonObject.Culture;
        existingVersion.Key = commonObject.Key;
        existingVersion.Version = 2;

        currentUser = actor;

        commonObjectServiceMock
            .Setup(expression: x => x.GetAllCommonObject())
            .Returns(value: new[] { existingVersion }.AsQueryable());

        commonObjectServiceMock
            .Setup(expression: x => x.AddCommonObjectAsync(newCommonObject: It.IsAny<CommonObject>()))
            .ReturnsAsync(valueFunction: (CommonObject item) => item);

        // When

        CommonObject result =
            await commonObjectProcessingService.UpdateCommonObjectAsync(updatedCommonObject: commonObject);

        // Then

        result.Id.Should()
            .Be(expected: 0);

        result.Version.Should()
            .Be(expected: 3);

        result.CreatedBy.Should()
            .Be(expected: actor.Id);

        result.LastUpdatedBy.Should()
            .Be(expected: actor.Id);

        commonObjectServiceMock.Verify(expression: x => x.GetAllCommonObject(), times: Times.Exactly(callCount: 2));

        commonObjectServiceMock.Verify(
expression: x =>
                x.AddCommonObjectAsync(
newCommonObject: It.Is<CommonObject>(match: item =>
                        item.Id == 0 && item.Version == 3
                    )
                ),
times: Times.Once
        );

        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}