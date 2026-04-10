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
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class LayoutServiceTests
{
    private readonly Mock<ILayoutBroker> layoutBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly LayoutService layoutService;

    public LayoutServiceTests()
    {
        layoutBrokerMock = new Mock<ILayoutBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        layoutService = new LayoutService(layoutBrokerMock.Object, authorizationBrokerMock.Object);
    }

    private static Layout CreateRandomLayout(int id = 42, int appId = 7)
    {
        Layout layout = Builder<Layout>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.AppId = appId)
            .With(x => x.HeaderHtml = "<header>Header</header>")
            .With(x => x.Html = "<main>Layout</main>")
            .With(x => x.Script = "console.log('layout');")
            .With(x => x.Name = $"Layout-{Guid.NewGuid():N}")
            .With(x => x.CreatedBy = "tester")
            .With(x => x.LastUpdatedBy = "tester")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5))
            .With(x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return layout;
    }
}




















