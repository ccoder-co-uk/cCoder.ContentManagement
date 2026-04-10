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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Script script = CreateRandomScript(id: 9, appId: 7);

        scriptBrokerMock.Setup(x => x.GetAllScripts(false)).Returns(new[] { script }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Script_delete"));
        scriptBrokerMock.Setup(x => x.DeleteScriptAsync(It.IsAny<CmsDataModels.Script>())).ReturnsAsync(1);

        // When
        await scriptService.DeleteAsync(9);

        // Then
        scriptBrokerMock.Verify(x => x.GetAllScripts(false), Times.Once);
        scriptBrokerMock.Verify(x => x.DeleteScriptAsync(It.Is<CmsDataModels.Script>(actual => actual.Id == script.Id)), Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Script_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Script script = CreateRandomScript(id: 9, appId: 7);

        scriptBrokerMock.Setup(x => x.GetAllScripts(false)).Returns(new[] { script }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Script_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await scriptService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        scriptBrokerMock.Verify(x => x.GetAllScripts(false), Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Script_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















