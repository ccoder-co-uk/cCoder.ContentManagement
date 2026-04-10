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
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachItemWhenUserHasDeletePrivilegeForDeleteAllAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        User actor = TestUsers.WithPrivilege("commonobject_delete");
        CommonObject first = CreateRandomCommonObject();
        CommonObject second = CreateRandomCommonObject();
        currentUser = actor;

        commonObjectServiceMock
            .Setup(x => x.DeleteAsync(first.Id))
            .Returns(ValueTask.CompletedTask);

        commonObjectServiceMock
            .Setup(x => x.DeleteAsync(second.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await commonObjectProcessingService.DeleteAllAsync(new[] { first, second });

        // Then
        commonObjectServiceMock.Verify(x => x.DeleteAsync(first.Id), Times.Once);
        commonObjectServiceMock.Verify(x => x.DeleteAsync(second.Id), Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}






















