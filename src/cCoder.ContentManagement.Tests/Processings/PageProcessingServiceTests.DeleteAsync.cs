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

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUserCanDeletePageForDeleteAsync()
    {
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

        User user = TestUsers.WithPrivilege("page_delete", 1);
        Page page = CreateRandomPage(user);
        currentUser = user;
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { page }.AsQueryable());
        pageServiceMock.Setup(x => x.DeleteAsync(page.Id)).Returns(ValueTask.CompletedTask);

        await pageProcessingService.DeleteAsync(page.Id);

        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.Verify(x => x.DeleteAsync(page.Id), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
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

        Page page = CreateRandomPage();
        currentUser = TestUsers.WithoutPrivileges();
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { page }.AsQueryable());

        Func<Task> act = async () => await pageProcessingService.DeleteAsync(page.Id);

        await act.Should().ThrowAsync<SecurityException>();
        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}










