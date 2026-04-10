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
    public void ShouldReturnDirectChildrenWhenGetChildren()
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

        Page parent = CreateRandomPage();
        Page child = CreateRandomPage();
        child.Id = 10;
        child.ParentId = parent.Id;
        pageServiceMock.Setup(x => x.GetAll()).Returns(new[] { parent, child }.AsQueryable());

        Page[] result = pageProcessingService.GetChildren(parent.Id).ToArray();

        result.Should().ContainSingle();
        result[0].Id.Should().Be(child.Id);
        pageServiceMock.Verify(x => x.GetAll(), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnEmptyCollectionWhenParentHasNoChildrenForGetChildren()
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

        Page parent = CreateRandomPage();
        Page other = CreateRandomPage();
        other.ParentId = parent.Id + 1;
        pageServiceMock.Setup(x => x.GetAll()).Returns(new[] { parent, other }.AsQueryable());

        Page[] result = pageProcessingService.GetChildren(parent.Id).ToArray();

        result.Should().BeEmpty();
        pageServiceMock.Verify(x => x.GetAll(), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}










