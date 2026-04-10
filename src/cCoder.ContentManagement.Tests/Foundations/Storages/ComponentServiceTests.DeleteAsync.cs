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

        componentBrokerMock.Setup(x => x.GetAllComponents(false)).Returns(new[] { component }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Component_delete"));
        componentBrokerMock.Setup(x => x.DeleteComponentAsync(It.IsAny<CmsDataModels.Component>())).ReturnsAsync(1);

        // When
        await componentService.DeleteAsync(9);

        // Then
        componentBrokerMock.Verify(x => x.GetAllComponents(false), Times.Once);
        componentBrokerMock.Verify(x => x.DeleteComponentAsync(It.Is<CmsDataModels.Component>(actual => actual.Id == component.Id)), Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Component_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Component component = CreateRandomComponent(id: 9, appId: 7);

        componentBrokerMock.Setup(x => x.GetAllComponents(false)).Returns(new[] { component }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Component_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await componentService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        componentBrokerMock.Verify(x => x.GetAllComponents(false), Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Component_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















