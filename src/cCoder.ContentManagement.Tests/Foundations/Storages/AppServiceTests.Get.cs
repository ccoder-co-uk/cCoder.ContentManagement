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
using System.Security;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    [Fact]
    public void ShouldReturnAppWhenGet()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        appBrokerMock.Setup(x => x.GetAllApps(false)).Returns(new[] { app }.AsQueryable());

        // When
        App result = appService.Get(5);

        // Then
        result.Should().BeEquivalentTo(app);
        appBrokerMock.Verify(x => x.GetAllApps(false), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnAppWhenGetIgnoringFilters()
    {
        App app = CreateRandomApp(id: 7);

        appBrokerMock.Setup(x => x.GetAllApps(true)).Returns(new[] { app }.AsQueryable());

        App result = appService.Get(7, ignoreFilters: true);

        result.Should().BeEquivalentTo(app);
        appBrokerMock.Verify(x => x.GetAllApps(true), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenAppExistsOnlyInIgnoredQuery()
    {
        App app = CreateRandomApp(id: 9);

        appBrokerMock.Setup(x => x.GetAllApps(false)).Returns(Array.Empty<cCoder.Data.Models.CMS.App>().AsQueryable());
        appBrokerMock.Setup(x => x.GetAllApps(true)).Returns(new[] { app }.AsQueryable());

        Action act = () => appService.Get(9);

        act.Should().Throw<SecurityException>().WithMessage("Access Denied!");
        appBrokerMock.Verify(x => x.GetAllApps(false), Times.Once);
        appBrokerMock.Verify(x => x.GetAllApps(true), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}


















