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
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachItemWhenUserHasDeletePrivilegeForDeleteAllAsync()
    {
        // Given
        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivilege(privilege: "commonobject_delete");
        CommonObject first = CreateRandomCommonObject();
        CommonObject second = CreateRandomCommonObject();
        currentUser = actor;

        commonObjectServiceMock
            .Setup(expression: x => x.DeleteAsync(commonObjectId: first.Id))
            .Returns(value: ValueTask.CompletedTask);

        commonObjectServiceMock
            .Setup(expression: x => x.DeleteAsync(commonObjectId: second.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await commonObjectProcessingService.DeleteAllCommonObjectAsync(deletedCommonObject: new[] { first, second });

        // Then
        commonObjectServiceMock.Verify(expression: x => x.DeleteAsync(commonObjectId: first.Id), times: Times.Once);
        commonObjectServiceMock.Verify(expression: x => x.DeleteAsync(commonObjectId: second.Id), times: Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}