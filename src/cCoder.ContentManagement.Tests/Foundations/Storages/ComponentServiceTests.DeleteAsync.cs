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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ComponentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Component component = CreateRandomComponent(id: 9, appId: 7);

        componentBrokerMock.Setup(expression: x => x.GetAllComponents(ignoreFilters: false))
            .Returns(value: new[] { component }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Component_delete"));

        componentBrokerMock.Setup(expression: x => x.DeleteComponentAsync(deletedComponent: It.IsAny<CmsDataModels.Component>()))
            .ReturnsAsync(value: 1);

        // When
        await componentService.DeleteAsync(componentId: 9);

        // Then
        componentBrokerMock.Verify(expression: x => x.GetAllComponents(ignoreFilters: false), times: Times.Once);
        componentBrokerMock.Verify(expression: x => x.DeleteComponentAsync(deletedComponent: It.Is<CmsDataModels.Component>(match: actual => actual.Id == component.Id)), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Component_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Component component = CreateRandomComponent(id: 9, appId: 7);

        componentBrokerMock.Setup(expression: x => x.GetAllComponents(ignoreFilters: false))
            .Returns(value: new[] { component }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Component_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await componentService.DeleteAsync(componentId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        componentBrokerMock.Verify(expression: x => x.GetAllComponents(ignoreFilters: false), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Component_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}