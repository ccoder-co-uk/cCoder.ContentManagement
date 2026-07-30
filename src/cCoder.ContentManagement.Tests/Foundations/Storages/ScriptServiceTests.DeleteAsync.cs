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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Script script = CreateRandomScript(id: 9, appId: 7);

        scriptBrokerMock.Setup(expression: x => x.GetAllScripts())
            .Returns(value: new[] { script }.AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_delete"));

        scriptBrokerMock.Setup(expression: x => x.DeleteScriptAsync(deletedScript: It.IsAny<CmsDataModels.Script>()))
            .ReturnsAsync(value: 1);

        // When
        await scriptService.DeleteAsync(scriptId: 9);

        // Then
        scriptBrokerMock.Verify(expression: x => x.GetAllScripts(), times: Times.Once);
        scriptBrokerMock.Verify(expression: x => x.DeleteScriptAsync(deletedScript: It.Is<CmsDataModels.Script>(match: actual => actual.Id == script.Id)), times: Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Script script = CreateRandomScript(id: 9, appId: 7);

        scriptBrokerMock.Setup(expression: x => x.GetAllScripts())
            .Returns(value: new[] { script }.AsQueryable());

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await scriptService.DeleteAsync(scriptId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        scriptBrokerMock.Verify(expression: x => x.GetAllScripts(), times: Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Script_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}